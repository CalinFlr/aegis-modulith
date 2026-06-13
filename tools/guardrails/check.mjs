#!/usr/bin/env node

import { existsSync, mkdirSync, readFileSync, readdirSync, statSync, writeFileSync } from "node:fs";
import { extname, join, relative } from "node:path";
import { spawn } from "node:child_process";
import { randomUUID } from "node:crypto";

const root = process.cwd();
const target = process.argv[2] ?? "all";

function fail(name, errors) {
  return { name, ok: false, errors };
}

function pass(name) {
  return { name, ok: true, errors: [] };
}

function read(path) {
  return readFileSync(join(root, path), "utf8");
}

function readAbsolute(path) {
  return readFileSync(path, "utf8");
}

function listFiles(dir, predicate = () => true) {
  const abs = join(root, dir);
  if (!existsSync(abs)) return [];
  const out = [];
  const ignoredDirectories = new Set([".git", "artifacts", "bin", "node_modules", "obj"]);
  function walk(current) {
    for (const entry of readdirSync(current)) {
      if (ignoredDirectories.has(entry)) continue;
      const full = join(current, entry);
      const st = statSync(full);
      if (st.isDirectory()) walk(full);
      else if (predicate(full)) out.push(relative(root, full));
    }
  }
  walk(abs);
  return out;
}

function listFilesAbsolute(dir, predicate = () => true) {
  if (!existsSync(dir)) return [];
  const out = [];
  function walk(current) {
    for (const entry of readdirSync(current)) {
      const full = join(current, entry);
      const st = statSync(full);
      if (st.isDirectory()) walk(full);
      else if (predicate(full)) out.push(full);
    }
  }
  walk(dir);
  return out;
}

async function runCommand(command, args, options = {}) {
  return new Promise((resolve) => {
    const child = spawn(command, args, {
      cwd: options.cwd ?? root,
      env: options.env ?? process.env,
      stdio: "inherit",
      shell: false
    });
    child.on("exit", code => resolve(code ?? 1));
    child.on("error", () => resolve(1));
  });
}

function assertExists(errors, path, message) {
  if (!existsSync(path)) {
    errors.push(message);
  }
}

function assertMissing(errors, path, message) {
  if (existsSync(path)) {
    errors.push(message);
  }
}

function assertContains(errors, path, expected, message) {
  if (!existsSync(path)) {
    errors.push(`${message} Missing file: ${path}.`);
    return;
  }

  const content = readAbsolute(path);
  if (!content.includes(expected)) {
    errors.push(message);
  }
}

function assertNotContains(errors, path, unexpected, message) {
  if (!existsSync(path)) {
    return;
  }

  const content = readAbsolute(path);
  if (content.includes(unexpected)) {
    errors.push(message);
  }
}

function assertGeneratedOptions(errors, output, variant) {
  const props = join(output, "Directory.Build.props");
  assertContains(errors, props, `<AegisProfile>${variant.profile}</AegisProfile>`, `${variant.id} should record the selected profile.`);
  assertContains(errors, props, `<AegisMediator>${variant.mediator}</AegisMediator>`, `${variant.id} should record the selected mediator.`);
  assertContains(errors, props, `<AegisSample>${variant.sample}</AegisSample>`, `${variant.id} should record the selected sample.`);
  assertContains(errors, props, `<AegisAi>${variant.ai}</AegisAi>`, `${variant.id} should record the selected AI mode.`);
  assertContains(errors, props, `<AegisGuardrails>${variant.guardrails}</AegisGuardrails>`, `${variant.id} should record the selected guardrails mode.`);
  assertContains(errors, props, `<AegisHooks>${variant.hooks}</AegisHooks>`, `${variant.id} should record the selected hooks mode.`);
  assertContains(errors, props, `<AegisSkills>${variant.skills}</AegisSkills>`, `${variant.id} should record the selected skills mode.`);
  assertContains(errors, props, `<AegisDocs>${variant.docs}</AegisDocs>`, `${variant.id} should record the selected docs mode.`);
  assertContains(errors, props, `<AegisLicense>${variant.licenseExpression}</AegisLicense>`, `${variant.id} should record the selected license.`);
}

function assertMediatorSemantics(errors, output, variant) {
  const dispatching = join(output, "src", `${variant.name}.BuildingBlocks`, "Cqrs", "DispatchingServiceCollectionExtensions.cs");
  const module = variant.sample === "taskhub" ? "Projects" : "WorkItems";
  const feature = variant.sample === "taskhub" ? "CreateProject" : "CreateWorkItem";
  const command = join(output, "src", `${variant.name}.Modules`, "Modules", module, "Features", feature, `${feature}Command.cs`);
  const handler = join(output, "src", `${variant.name}.Modules`, "Modules", module, "Features", feature, `${feature}Handler.cs`);

  if (variant.mediator === "mediatr") {
    assertContains(errors, dispatching, "services.AddMediatR", `${variant.id} should register MediatR services.`);
    assertContains(errors, dispatching, "MediatRCommandDispatcher", `${variant.id} should use the MediatR command dispatcher.`);
    assertContains(errors, dispatching, "MediatRQueryDispatcher", `${variant.id} should use the MediatR query dispatcher.`);
    assertContains(errors, dispatching, "ISender sender", `${variant.id} should dispatch through MediatR ISender.`);
    assertNotContains(errors, dispatching, "RegisterCoreHandlers(services", `${variant.id} should not register core handlers in MediatR mode.`);
    assertNotContains(errors, dispatching, "ServiceProviderCommandDispatcher", `${variant.id} should not use the core command dispatcher in MediatR mode.`);
    assertContains(errors, command, "MediatR.IRequest<", `${variant.id} commands should implement MediatR.IRequest.`);
    assertContains(errors, handler, "MediatR.IRequestHandler<", `${variant.id} handlers should implement MediatR.IRequestHandler.`);
    return;
  }

  assertContains(errors, dispatching, "RegisterCoreHandlers(services", `${variant.id} should register core CQRS handlers.`);
  assertContains(errors, dispatching, "ServiceProviderCommandDispatcher", `${variant.id} should use the core command dispatcher.`);
  assertContains(errors, dispatching, "ServiceProviderQueryDispatcher", `${variant.id} should use the core query dispatcher.`);
  assertNotContains(errors, dispatching, "services.AddMediatR", `${variant.id} should not register MediatR services in core mediator mode.`);
  assertNotContains(errors, dispatching, "MediatRCommandDispatcher", `${variant.id} should not use the MediatR dispatcher in core mediator mode.`);
  assertNotContains(errors, command, "MediatR.IRequest<", `${variant.id} commands should not implement MediatR.IRequest in core mediator mode.`);
  assertNotContains(errors, handler, "MediatR.IRequestHandler<", `${variant.id} handlers should not implement MediatR.IRequestHandler in core mediator mode.`);
}

function assertProfileSemantics(errors, output, variant) {
  const solution = join(output, `${variant.name}.sln`);
  const program = join(output, "src", `${variant.name}.Api`, "Program.cs");
  const apiProject = join(output, "src", `${variant.name}.Api`, `${variant.name}.Api.csproj`);
  const appHostProject = join(output, "src", `${variant.name}.AppHost`, `${variant.name}.AppHost.csproj`);
  const serviceDefaultsProject = join(output, "src", `${variant.name}.ServiceDefaults`, `${variant.name}.ServiceDefaults.csproj`);
  const dockerfile = join(output, "Dockerfile");
  const proServices = join(output, "src", `${variant.name}.Api`, "Pro", "ProProfileServices.cs");
  const advancedServices = join(output, "src", `${variant.name}.Api`, "Advanced", "AdvancedProfileServices.cs");

  if (variant.profile === "core") {
    assertMissing(errors, appHostProject, `${variant.id} core profile should not include AppHost.`);
    assertMissing(errors, serviceDefaultsProject, `${variant.id} core profile should not include ServiceDefaults.`);
    assertMissing(errors, dockerfile, `${variant.id} core profile should not include Dockerfile.`);
    assertMissing(errors, proServices, `${variant.id} core profile should not include pro profile services.`);
    assertMissing(errors, advancedServices, `${variant.id} core profile should not include advanced profile services.`);
    assertNotContains(errors, apiProject, "ServiceDefaults", `${variant.id} API project should not reference ServiceDefaults.`);
    assertNotContains(errors, solution, "AppHost", `${variant.id} solution should not reference AppHost.`);
    assertNotContains(errors, solution, "ServiceDefaults", `${variant.id} solution should not reference ServiceDefaults.`);
    assertNotContains(errors, program, "AddProProfileServices", `${variant.id} Program.cs should not wire pro services.`);
    assertNotContains(errors, program, "MapProProfileEndpoints", `${variant.id} Program.cs should not map pro endpoints.`);
    assertNotContains(errors, program, "AddAdvancedProfileServices", `${variant.id} Program.cs should not wire advanced services.`);
    assertNotContains(errors, program, "MapAdvancedProfileEndpoints", `${variant.id} Program.cs should not map advanced endpoints.`);
    return;
  }

  assertExists(errors, appHostProject, `${variant.id} should include AppHost.`);
  assertExists(errors, serviceDefaultsProject, `${variant.id} should include ServiceDefaults.`);
  assertExists(errors, dockerfile, `${variant.id} should include Dockerfile.`);
  assertExists(errors, proServices, `${variant.id} should include pro profile services.`);
  assertContains(errors, apiProject, "ServiceDefaults", `${variant.id} API project should reference ServiceDefaults.`);
  assertContains(errors, solution, "AppHost", `${variant.id} solution should reference AppHost.`);
  assertContains(errors, solution, "ServiceDefaults", `${variant.id} solution should reference ServiceDefaults.`);
  assertContains(errors, program, "builder.AddServiceDefaults();", `${variant.id} Program.cs should wire ServiceDefaults.`);
  assertContains(errors, program, "builder.Services.AddProProfileServices(builder.Configuration);", `${variant.id} Program.cs should register pro services.`);
  assertContains(errors, program, "app.UseRateLimiter();", `${variant.id} Program.cs should enable rate limiting middleware.`);
  assertContains(errors, program, "app.MapProProfileEndpoints();", `${variant.id} Program.cs should map pro endpoints.`);

  if (variant.profile === "advanced") {
    assertExists(errors, advancedServices, `${variant.id} should include advanced profile services.`);
    assertContains(errors, program, "builder.Services.AddAdvancedProfileServices();", `${variant.id} Program.cs should register advanced services.`);
    assertContains(errors, program, "app.MapAdvancedProfileEndpoints();", `${variant.id} Program.cs should map advanced endpoints.`);
  } else {
    assertMissing(errors, advancedServices, `${variant.id} pro profile should not include advanced profile services.`);
    assertNotContains(errors, program, "AddAdvancedProfileServices", `${variant.id} pro Program.cs should not wire advanced services.`);
    assertNotContains(errors, program, "MapAdvancedProfileEndpoints", `${variant.id} pro Program.cs should not map advanced endpoints.`);
  }
}

function assertP1DFeatureDepthSemantics(errors, output, variant) {
  const solution = join(output, `${variant.name}.sln`);
  const packages = join(output, "Directory.Packages.props");
  const program = join(output, "src", `${variant.name}.Api`, "Program.cs");
  const apiProject = join(output, "src", `${variant.name}.Api`, `${variant.name}.Api.csproj`);
  const integrationRoot = join(output, "tests", `${variant.name}.IntegrationTests`);
  const integrationProject = join(integrationRoot, `${variant.name}.IntegrationTests.csproj`);
  const postgresFixture = join(integrationRoot, "Infrastructure", "PostgresContainerFixture.cs");
  const dockerFact = join(integrationRoot, "Infrastructure", "DockerFactAttribute.cs");
  const factory = join(integrationRoot, "Infrastructure", "AegisWebApplicationFactory.cs");
  const databaseInitialization = join(integrationRoot, "Infrastructure", "DatabaseInitialization.cs");
  const containerSmoke = join(integrationRoot, "ContainerizedPostgresSmokeTests.cs");
  const fakeAuthHandler = join(integrationRoot, "Authentication", "FakeAuthenticationHandler.cs");
  const fakeAuthDefaults = join(integrationRoot, "Authentication", "FakeAuthenticationDefaults.cs");
  const fakeAuthHeaders = join(integrationRoot, "Authentication", "FakeAuthenticationHeaders.cs");
  const authenticatedClientExtensions = join(integrationRoot, "Authentication", "AuthenticatedClientExtensions.cs");
  const fakeAuthSmoke = join(integrationRoot, "FakeAuthenticationSmokeTests.cs");
  const proHttpRoot = join(output, "src", `${variant.name}.Api`, "Pro", "Http");
  const resilientHttp = join(proHttpRoot, "ResilientHttpClientServiceCollectionExtensions.cs");
  const sampleClient = join(proHttpRoot, "SampleExternalStatusClient.cs");
  const proServices = join(output, "src", `${variant.name}.Api`, "Pro", "ProProfileServices.cs");
  const testingDocs = join(output, "docs", "testing.md");

  for (const forbidden of ["FakeAuthenticationHandler", "FakeAuthenticationDefaults", "X-Test-User-Id", "Aegis.Test"]) {
    assertNotContains(errors, program, forbidden, `${variant.id} production Program.cs must not wire fake authentication.`);
  }

  if (variant.profile === "core") {
    assertMissing(errors, integrationRoot, `${variant.id} core profile should not include the P1D integration test project.`);
    assertMissing(errors, proHttpRoot, `${variant.id} core profile should not include pro HttpClient resilience files.`);
    assertNotContains(errors, packages, "Testcontainers.PostgreSql", `${variant.id} core profile should not include Testcontainers package versions.`);
    assertNotContains(errors, packages, "Microsoft.AspNetCore.Mvc.Testing", `${variant.id} core profile should not include WebApplicationFactory package versions.`);
    assertNotContains(errors, packages, "Microsoft.Extensions.Http.Resilience", `${variant.id} core profile should not include HttpClient resilience package versions.`);
    assertNotContains(errors, apiProject, "Microsoft.Extensions.Http.Resilience", `${variant.id} core API project should not reference HttpClient resilience.`);
    assertNotContains(errors, solution, "IntegrationTests", `${variant.id} core solution should not include integration tests.`);
    assertContains(errors, testingDocs, "## Core Profile", `${variant.id} generated testing docs should describe the core test surface.`);
    assertNotContains(errors, testingDocs, `tests/${variant.name}.IntegrationTests`, `${variant.id} generated testing docs should not describe a missing integration test project.`);
    assertNotContains(errors, testingDocs, "## Pro And Advanced Integration Tests", `${variant.id} generated testing docs should not include pro/advanced integration-test guidance.`);
    return;
  }

  assertExists(errors, integrationProject, `${variant.id} pro/advanced profile should include generated integration tests.`);
  assertContains(errors, solution, `${variant.name}.IntegrationTests`, `${variant.id} solution should include integration tests so they build and default-skipped Docker tests are visible.`);
  assertContains(errors, packages, "Testcontainers.PostgreSql", `${variant.id} should include Testcontainers PostgreSQL package version.`);
  assertContains(errors, packages, "Microsoft.AspNetCore.Mvc.Testing", `${variant.id} should include WebApplicationFactory package version.`);
  assertContains(errors, integrationProject, "Testcontainers.PostgreSql", `${variant.id} integration tests should reference Testcontainers PostgreSQL.`);
  assertContains(errors, integrationProject, "Microsoft.AspNetCore.Mvc.Testing", `${variant.id} integration tests should reference Microsoft.AspNetCore.Mvc.Testing.`);

  assertContains(errors, postgresFixture, "PostgreSqlBuilder", `${variant.id} should include a PostgreSQL Testcontainers fixture.`);
  assertContains(errors, postgresFixture, "postgres:17-alpine", `${variant.id} PostgreSQL fixture should name the local Docker image.`);
  assertContains(errors, postgresFixture, "DisposeAsync", `${variant.id} PostgreSQL fixture should dispose the container cleanly.`);
  assertContains(errors, dockerFact, "AEGIS_RUN_TESTCONTAINERS", `${variant.id} Docker-backed tests should be opt-in and documented in code.`);
  assertContains(errors, factory, "ConnectionStrings:Postgres", `${variant.id} integration factory should override the generated PostgreSQL connection string.`);
  assertContains(errors, databaseInitialization, "Database.MigrateAsync", `${variant.id} database initialization placeholder should document migration handoff.`);
  assertContains(errors, containerSmoke, "Api_host_starts_against_containerized_postgres", `${variant.id} should include an API host smoke test against containerized PostgreSQL.`);
  assertContains(errors, containerSmoke, "DatabaseInitialization.InitializeAsync", `${variant.id} container smoke test should exercise the database initialization path.`);

  assertExists(errors, fakeAuthHandler, `${variant.id} should include fake auth test handler.`);
  assertContains(errors, fakeAuthDefaults, "AuthenticationScheme = \"Aegis.Test\"", `${variant.id} fake auth should use a clearly named test scheme.`);
  assertContains(errors, fakeAuthDefaults, "X-Test-User-Id", `${variant.id} fake auth should document user header claims.`);
  assertContains(errors, fakeAuthDefaults, "X-Test-Roles", `${variant.id} fake auth should document test roles.`);
  assertContains(errors, fakeAuthDefaults, "X-Test-Scopes", `${variant.id} fake auth should document test scopes.`);
  assertContains(errors, fakeAuthHandler, "AuthenticationHandler<AuthenticationSchemeOptions>", `${variant.id} fake auth should use an ASP.NET Core authentication handler.`);
  assertContains(errors, fakeAuthHeaders, "ReadCsvHeader", `${variant.id} fake auth headers should support role and scope mapping inputs.`);
  assertContains(errors, authenticatedClientExtensions, "CreateAuthenticatedClient", `${variant.id} should include authenticated test client helpers.`);
  assertContains(errors, fakeAuthSmoke, "Fake_authentication_scheme_maps_test_client_headers_to_claims", `${variant.id} should include a fake auth mechanism smoke test.`);
  assertContains(errors, factory, "enableFakeAuthentication", `${variant.id} fake auth should be enabled only through the test factory.`);

  assertContains(errors, packages, "Microsoft.Extensions.Http.Resilience", `${variant.id} should include Microsoft HttpClient resilience package version.`);
  assertContains(errors, apiProject, "Microsoft.Extensions.Http.Resilience", `${variant.id} API project should reference Microsoft HttpClient resilience.`);
  assertExists(errors, resilientHttp, `${variant.id} should include HttpClient resilience registration extension.`);
  assertContains(errors, resilientHttp, "ConfigureHttpClientDefaults", `${variant.id} should configure default outbound HttpClient behavior.`);
  assertContains(errors, resilientHttp, "AddStandardResilienceHandler", `${variant.id} should use the Microsoft standard resilience handler.`);
  assertContains(errors, resilientHttp, "AddHttpClient<SampleExternalStatusClient>", `${variant.id} should include a typed sample external client registration.`);
  assertContains(errors, resilientHttp, "https://example.invalid/", `${variant.id} sample external client should not depend on a real external API.`);
  assertContains(errors, proServices, "AddAegisOutboundHttpClients", `${variant.id} pro services should wire HttpClient resilience defaults.`);
  assertContains(errors, testingDocs, `tests/${variant.name}.IntegrationTests`, `${variant.id} generated testing docs should name the generated integration test project.`);
  assertContains(errors, testingDocs, "## Fake Authentication", `${variant.id} generated testing docs should include fake authentication test guidance.`);
  assertContains(errors, testingDocs, "## HttpClient Resilience", `${variant.id} generated testing docs should include HttpClient resilience guidance.`);
  assertNotContains(errors, testingDocs, "## Core Profile", `${variant.id} generated testing docs should not include the core-only section.`);
}

function assertP1D2AAuthPermissionSemantics(errors, output, variant) {
  const packages = join(output, "Directory.Packages.props");
  const program = join(output, "src", `${variant.name}.Api`, "Program.cs");
  const apiProject = join(output, "src", `${variant.name}.Api`, `${variant.name}.Api.csproj`);
  const proAuthRoot = join(output, "src", `${variant.name}.Api`, "Pro", "Auth");
  const jwtOptions = join(proAuthRoot, "AegisJwtOptions.cs");
  const jwtRegistration = join(proAuthRoot, "AegisJwtAuthenticationServiceCollectionExtensions.cs");
  const permissionRegistration = join(proAuthRoot, "AegisPermissionPolicyServiceCollectionExtensions.cs");
  const permissionConstants = join(output, "src", `${variant.name}.BuildingBlocks`, "Authorization", "AegisPermissions.cs");
  const policyConstants = join(output, "src", `${variant.name}.BuildingBlocks`, "Authorization", "AegisAuthorizationPolicies.cs");
  const claimTypes = join(output, "src", `${variant.name}.BuildingBlocks`, "Authorization", "AegisPermissionClaimTypes.cs");
  const proServices = join(output, "src", `${variant.name}.Api`, "Pro", "ProProfileServices.cs");
  const advancedServices = join(output, "src", `${variant.name}.Api`, "Advanced", "AdvancedProfileServices.cs");
  const workItemsModule = join(output, "src", `${variant.name}.Modules`, "Modules", "WorkItems", "WorkItemsModule.cs");
  const tasksModule = join(output, "src", `${variant.name}.Modules`, "Modules", "Tasks", "TasksModule.cs");
  const integrationRoot = join(output, "tests", `${variant.name}.IntegrationTests`);
  const fakeAuthDefaults = join(integrationRoot, "Authentication", "FakeAuthenticationDefaults.cs");
  const fakeAuthHandler = join(integrationRoot, "Authentication", "FakeAuthenticationHandler.cs");
  const authenticatedClientExtensions = join(integrationRoot, "Authentication", "AuthenticatedClientExtensions.cs");
  const permissionAuthorizationTests = join(integrationRoot, "PermissionAuthorizationTests.cs");

  if (variant.profile === "core") {
    assertMissing(errors, proAuthRoot, `${variant.id} core profile should not include pro JWT/auth registration assets.`);
    assertNotContains(errors, packages, "Microsoft.AspNetCore.Authentication.JwtBearer", `${variant.id} core profile should not include JWT bearer package versions.`);
    assertNotContains(errors, apiProject, "Microsoft.AspNetCore.Authentication.JwtBearer", `${variant.id} core API project should not reference JWT bearer.`);
    assertNotContains(errors, program, "UseAuthentication", `${variant.id} core Program.cs should not wire authentication middleware.`);
    assertNotContains(errors, program, "UseAuthorization", `${variant.id} core Program.cs should not wire authorization middleware.`);
    assertNotContains(errors, workItemsModule, "RequireAuthorization", `${variant.id} core starter module should not require pro/advanced policies.`);
    assertExists(errors, permissionConstants, `${variant.id} core output should keep only minimal shared permission constants.`);
    assertMissing(errors, join(output, "src", `${variant.name}.Api`, "Advanced", "Permissions"), `${variant.id} core profile should not include advanced permission placeholders.`);
    return;
  }

  assertContains(errors, packages, "Microsoft.AspNetCore.Authentication.JwtBearer", `${variant.id} should include JWT bearer package version.`);
  assertContains(errors, apiProject, "Microsoft.AspNetCore.Authentication.JwtBearer", `${variant.id} API project should reference JWT bearer.`);
  assertExists(errors, jwtOptions, `${variant.id} should include strongly typed JWT options.`);
  assertContains(errors, jwtOptions, "SectionName = \"Authentication:Jwt\"", `${variant.id} JWT options should name the configuration section.`);
  assertContains(errors, jwtOptions, "Issuer", `${variant.id} JWT options should include issuer configuration.`);
  assertContains(errors, jwtOptions, "Audience", `${variant.id} JWT options should include audience configuration.`);
  assertContains(errors, jwtOptions, "SigningKey", `${variant.id} JWT options should include signing-key configuration.`);
  assertContains(errors, jwtRegistration, "AddJwtBearer", `${variant.id} should register JWT bearer authentication.`);
  assertContains(errors, jwtRegistration, "CreateRejectAllValidationParameters", `${variant.id} missing JWT config should reject tokens by default.`);
  assertContains(errors, jwtRegistration, "RandomNumberGenerator.GetBytes", `${variant.id} reject-all JWT validation should not use a hardcoded signing key.`);
  assertContains(errors, jwtRegistration, "RequireSignedTokens = true", `${variant.id} JWT validation should require signed tokens.`);
  assertContains(errors, proServices, "AddAegisJwtAuthentication(configuration)", `${variant.id} pro services should wire JWT authentication.`);

  if (existsSync(program)) {
    const content = readAbsolute(program);
    const authIndex = content.indexOf("app.UseAuthentication();");
    const authzIndex = content.indexOf("app.UseAuthorization();");
    if (authIndex === -1 || authzIndex === -1 || authIndex > authzIndex) {
      errors.push(`${variant.id} Program.cs should call UseAuthentication before UseAuthorization.`);
    }
  }

  assertExists(errors, permissionConstants, `${variant.id} should include permission constants.`);
  assertExists(errors, policyConstants, `${variant.id} should include authorization policy constants.`);
  assertExists(errors, claimTypes, `${variant.id} should include permission claim-type constants.`);
  assertContains(errors, permissionConstants, "TasksRead = \"tasks:read\"", `${variant.id} should include TaskHub task read permission.`);
  assertContains(errors, permissionConstants, "TasksWrite = \"tasks:write\"", `${variant.id} should include TaskHub task write permission.`);
  assertContains(errors, permissionRegistration, "AddAegisPermissionPolicies", `${variant.id} should include permission policy registration.`);
  assertContains(errors, permissionRegistration, "AegisPermissionClaimTypes.Permission", `${variant.id} permission policies should use permission claims.`);
  assertContains(errors, permissionRegistration, "AegisPermissionClaimTypes.Scope", `${variant.id} permission policies should accept scope claims for compatibility.`);
  assertContains(errors, proServices, "AddAegisPermissionPolicies", `${variant.id} pro services should wire permission policies.`);
  assertContains(errors, proServices, "RequireAuthorization", `${variant.id} should protect at least one pro endpoint.`);
  assertContains(errors, proServices, "AegisAuthorizationPolicies.OperationsRead", `${variant.id} should protect at least one pro endpoint with a named policy.`);

  if (variant.sample === "taskhub") {
    assertContains(errors, tasksModule, "RequireAuthorization(AegisAuthorizationPolicies.TasksRead)", `${variant.id} TaskHub list endpoint should require task read permission.`);
    assertContains(errors, tasksModule, "RequireAuthorization(AegisAuthorizationPolicies.TasksWrite)", `${variant.id} TaskHub create endpoint should require task write permission.`);
  } else {
    assertContains(errors, workItemsModule, "RequireAuthorization(AegisAuthorizationPolicies.WorkItemsRead)", `${variant.id} starter read endpoint should require work-item read permission.`);
    assertContains(errors, workItemsModule, "RequireAuthorization(AegisAuthorizationPolicies.WorkItemsWrite)", `${variant.id} starter write endpoint should require work-item write permission.`);
  }

  if (variant.profile === "advanced") {
    assertContains(errors, advancedServices, "RequireAuthorization", `${variant.id} advanced endpoint should require authorization.`);
    assertContains(errors, advancedServices, "AegisAuthorizationPolicies.AdvancedRead", `${variant.id} advanced endpoint should require a named advanced policy.`);
  }

  for (const forbidden of ["FakeAuthenticationHandler", "FakeAuthenticationDefaults", "X-Test-Permissions", "Aegis.Test"]) {
    assertNotContains(errors, program, forbidden, `${variant.id} production Program.cs must not wire fake authentication.`);
  }

  assertContains(errors, fakeAuthDefaults, "X-Test-Permissions", `${variant.id} fake auth should include permission headers for tests.`);
  assertContains(errors, fakeAuthHandler, "PermissionClaimType", `${variant.id} fake auth should emit permission claims.`);
  assertContains(errors, authenticatedClientExtensions, "AuthenticateWithPermissions", `${variant.id} should include permission-aware authenticated client helpers.`);
  assertContains(errors, permissionAuthorizationTests, "Request_with_required_permission_can_access_protected_endpoint", `${variant.id} should prove authorized permission requests pass.`);
  assertContains(errors, permissionAuthorizationTests, "Request_without_required_permission_is_forbidden", `${variant.id} should prove missing permissions are rejected.`);
}

function assertP1D2BInboxSemantics(errors, output, variant) {
  const packages = join(output, "Directory.Packages.props");
  const apiProject = join(output, "src", `${variant.name}.Api`, `${variant.name}.Api.csproj`);
  const inboxRoot = join(output, "src", `${variant.name}.Api`, "Pro", "Infrastructure", "Inbox");
  const inboxMessage = join(inboxRoot, "InboxMessage.cs");
  const inboxStatus = join(inboxRoot, "InboxMessageStatus.cs");
  const inboxConfiguration = join(inboxRoot, "InboxMessageConfiguration.cs");
  const inboxDbContext = join(inboxRoot, "AegisInboxDbContext.cs");
  const inboxStore = join(inboxRoot, "IInboxStore.cs");
  const efStore = join(inboxRoot, "EfCoreInboxStore.cs");
  const processor = join(inboxRoot, "InboxProcessor.cs");
  const processorWorker = join(inboxRoot, "InboxProcessorWorker.cs");
  const handler = join(inboxRoot, "SampleIntegrationEventInboxHandler.cs");
  const serviceRegistration = join(inboxRoot, "InboxServiceCollectionExtensions.cs");
  const proServices = join(output, "src", `${variant.name}.Api`, "Pro", "ProProfileServices.cs");
  const integrationRoot = join(output, "tests", `${variant.name}.IntegrationTests`);
  const integrationProject = join(integrationRoot, `${variant.name}.IntegrationTests.csproj`);
  const inboxTests = join(integrationRoot, "Inbox", "InboxStoreTests.cs");
  const messagingDocs = join(output, "docs", "messaging.md");

  if (variant.profile === "core") {
    assertMissing(errors, inboxRoot, `${variant.id} core profile should not include active inbox infrastructure.`);
    assertNotContains(errors, packages, "Microsoft.EntityFrameworkCore.InMemory", `${variant.id} core profile should not include inbox test package versions.`);
    assertNotContains(errors, apiProject, "Npgsql.EntityFrameworkCore.PostgreSQL", `${variant.id} core API project should not reference inbox persistence package directly.`);
    assertExists(errors, messagingDocs, `${variant.id} generated docs should include profile-accurate messaging guidance.`);
    assertContains(errors, messagingDocs, "## Core Profile", `${variant.id} core messaging docs should describe that inbox is excluded.`);
    assertContains(errors, messagingDocs, "Core does not generate active inbox infrastructure", `${variant.id} core messaging docs should state that active inbox infrastructure is excluded.`);
    assertNotContains(errors, messagingDocs, `tests/${variant.name}.IntegrationTests`, `${variant.id} core messaging docs should not reference a missing integration test project.`);
    return;
  }

  for (const [path, description] of [
    [inboxMessage, "inbox entity"],
    [inboxStatus, "inbox status enum"],
    [inboxConfiguration, "inbox EF configuration"],
    [inboxDbContext, "inbox DbContext"],
    [inboxStore, "inbox service abstraction"],
    [efStore, "EF Core inbox store"],
    [processor, "inbox processor"],
    [processorWorker, "optional inbox processor worker"],
    [handler, "sample integration-event inbox handler"],
    [serviceRegistration, "inbox service registration"],
    [inboxTests, "generated inbox behavior tests"]
  ]) {
    assertExists(errors, path, `${variant.id} missing ${description}.`);
  }

  assertContains(errors, apiProject, "Npgsql.EntityFrameworkCore.PostgreSQL", `${variant.id} API project should reference PostgreSQL EF support for inbox persistence.`);
  assertContains(errors, packages, "Microsoft.EntityFrameworkCore.InMemory", `${variant.id} should include EF InMemory package version for fast generated inbox tests.`);
  assertContains(errors, integrationProject, "Microsoft.EntityFrameworkCore.InMemory", `${variant.id} inbox tests should use EF InMemory instead of Docker.`);

  for (const marker of [
    "MessageId",
    "IdempotencyKey",
    "MessageType",
    "Payload",
    "ReceivedAtUtc",
    "ProcessedAtUtc",
    "FailureReason",
    "AttemptCount",
    "LockToken",
    "LockedUntilUtc"
  ]) {
    assertContains(errors, inboxMessage, marker, `${variant.id} inbox entity should include ${marker}.`);
  }
  for (const status of ["Pending", "Processing", "Processed", "Failed"]) {
    assertContains(errors, inboxStatus, status, `${variant.id} inbox status should include ${status}.`);
  }
  assertContains(errors, inboxConfiguration, "ToTable(\"inbox_messages\", AegisInboxDbContext.Schema)", `${variant.id} inbox table should use PostgreSQL-friendly naming and schema.`);
  assertContains(errors, inboxConfiguration, "HasIndex(message => message.MessageId).IsUnique()", `${variant.id} inbox should enforce unique MessageId.`);
  assertContains(errors, inboxConfiguration, "HasIndex(message => message.IdempotencyKey).IsUnique()", `${variant.id} inbox should enforce unique IdempotencyKey.`);
  assertContains(errors, inboxDbContext, "Schema = \"integration\"", `${variant.id} inbox DbContext should use the documented integration schema.`);
  assertContains(errors, inboxDbContext, "DbSet<InboxMessage>", `${variant.id} inbox DbContext should expose InboxMessages.`);

  for (const method of [
    "AcceptAsync",
    "IsDuplicateAsync",
    "TryBeginProcessingAsync",
    "MarkProcessedAsync",
    "MarkFailedAsync",
    "GetPendingAsync"
  ]) {
    assertContains(errors, inboxStore, method, `${variant.id} inbox abstraction should include ${method}.`);
  }
  assertContains(errors, efStore, "DbUpdateException", `${variant.id} inbox store should tolerate unique-constraint duplicate races.`);
  assertContains(errors, efStore, "InboxAcceptResult.Accepted", `${variant.id} inbox store should return accepted results.`);
  assertContains(errors, efStore, "InboxAcceptResult.Duplicate", `${variant.id} inbox store should return duplicate results.`);
  assertContains(errors, efStore, "InboxAcceptResult.AlreadyProcessed", `${variant.id} inbox store should return already-processed results.`);

  assertContains(errors, processor, "IEnumerable<IInboxMessageHandler>", `${variant.id} inbox processor should dispatch through handler abstraction.`);
  assertContains(errors, processor, "MarkProcessedAsync", `${variant.id} inbox processor should mark processed messages.`);
  assertContains(errors, processor, "MarkFailedAsync", `${variant.id} inbox processor should mark failed messages.`);
  assertContains(errors, serviceRegistration, "Inbox:EnableBackgroundProcessor", `${variant.id} inbox background processor should be opt-in by configuration.`);
  assertContains(errors, serviceRegistration, "AddHostedService<InboxProcessorWorker>", `${variant.id} inbox worker registration should be available when opted in.`);
  assertContains(errors, proServices, "AddAegisInbox(configuration)", `${variant.id} pro services should wire the inbox foundation.`);
  assertContains(errors, handler, ".Contracts", `${variant.id} sample inbox handler should use integration-event contracts.`);
  assertContains(errors, handler, "IntegrationEvent", `${variant.id} sample inbox handler should use an integration event type.`);
  assertNotContains(errors, handler, ".Domain", `${variant.id} sample inbox handler should not depend on domain entities.`);

  for (const testName of [
    "First_message_is_accepted",
    "Duplicate_message_id_is_detected",
    "Processed_message_is_not_accepted_again",
    "Failed_message_records_failure_and_retry_state",
    "Processor_invokes_handler_once_for_duplicate_inputs"
  ]) {
    assertContains(errors, inboxTests, testName, `${variant.id} inbox tests should cover ${testName}.`);
  }

  assertExists(errors, messagingDocs, `${variant.id} generated docs should include messaging guidance.`);
  assertContains(errors, messagingDocs, "## Pro And Advanced Profiles", `${variant.id} messaging docs should describe pro/advanced inbox behavior.`);
  assertContains(errors, messagingDocs, "MessageId", `${variant.id} messaging docs should explain MessageId idempotency.`);
  assertContains(errors, messagingDocs, "IdempotencyKey", `${variant.id} messaging docs should explain IdempotencyKey idempotency.`);
  assertContains(errors, messagingDocs, "Inbox:EnableBackgroundProcessor", `${variant.id} messaging docs should document opt-in processor execution.`);
  assertContains(errors, messagingDocs, "No broker is included by default", `${variant.id} messaging docs should state no broker is included.`);
  assertContains(errors, messagingDocs, "not event sourcing", `${variant.id} messaging docs should state inbox is not event sourcing.`);
  assertContains(errors, messagingDocs, `tests/${variant.name}.IntegrationTests`, `${variant.id} messaging docs should point to generated inbox tests.`);
}

function assertP1D3AContractTestSemantics(errors, output, variant) {
  const solution = join(output, `${variant.name}.sln`);
  const docs = join(output, "docs", "contracts.md");
  const contractRoot = join(output, "tests", `${variant.name}.ContractTests`);
  const contractProject = join(contractRoot, `${variant.name}.ContractTests.csproj`);
  const apiContractTests = join(contractRoot, "ApiContractTests.cs");
  const integrationEventContractTests = join(contractRoot, "IntegrationEventContractTests.cs");
  const inboxContractTests = join(contractRoot, "InboxContractTests.cs");
  const apiProject = join(output, "src", `${variant.name}.Api`, `${variant.name}.Api.csproj`);
  const modulesProject = join(output, "src", `${variant.name}.Modules`, `${variant.name}.Modules.csproj`);
  const buildingBlocksProject = join(output, "src", `${variant.name}.BuildingBlocks`, `${variant.name}.BuildingBlocks.csproj`);
  const integrationEventBase = join(output, "src", `${variant.name}.BuildingBlocks`, "Events", "IntegrationEvent.cs");
  const openApiRegistration = join(output, "src", `${variant.name}.Api`, "Pro", "Auth", "AegisOpenApiServiceCollectionExtensions.cs");
  const program = join(output, "src", `${variant.name}.Api`, "Program.cs");
  const sampleContract = variant.sample === "taskhub"
    ? join(output, "src", `${variant.name}.Modules`, "Modules", "Tasks", "Contracts", "TaskCreatedIntegrationEvent.cs")
    : join(output, "src", `${variant.name}.Modules`, "Modules", "WorkItems", "Contracts", "WorkItemCreatedIntegrationEvent.cs");

  assertExists(errors, docs, `${variant.id} should include generated contract testing docs.`);

  if (variant.profile === "core") {
    assertMissing(errors, contractRoot, `${variant.id} core profile should not include the pro/advanced contract test project.`);
    assertNotContains(errors, solution, "ContractTests", `${variant.id} core solution should not include contract tests.`);
    assertMissing(errors, openApiRegistration, `${variant.id} core profile should not include pro OpenAPI auth contract metadata.`);
    assertContains(errors, docs, "Core does not generate the pro/advanced contract test project", `${variant.id} contract docs should explain core exclusion.`);
    return;
  }

  assertExists(errors, contractProject, `${variant.id} pro/advanced profile should include generated contract tests.`);
  assertContains(errors, solution, `${variant.name}.ContractTests`, `${variant.id} solution should include contract tests so dotnet test runs them.`);
  if (existsSync(contractProject)) {
    const contractProjectContent = readAbsolute(contractProject).replaceAll("\\", "/");
    if (!contractProjectContent.includes(`${variant.name}.Api/${variant.name}.Api.csproj`)) {
      errors.push(`${variant.id} contract tests should reference the API project.`);
    }
    if (!contractProjectContent.includes(`${variant.name}.BuildingBlocks/${variant.name}.BuildingBlocks.csproj`)) {
      errors.push(`${variant.id} contract tests should reference BuildingBlocks.`);
    }
    if (!contractProjectContent.includes(`${variant.name}.Modules/${variant.name}.Modules.csproj`)) {
      errors.push(`${variant.id} contract tests should reference Modules.`);
    }
  }
  assertContains(errors, contractProject, "Microsoft.AspNetCore.Mvc.Testing", `${variant.id} contract tests should use WebApplicationFactory for OpenAPI metadata.`);
  assertNotContains(errors, contractProject, "Testcontainers.PostgreSql", `${variant.id} contract tests should not require Docker/Testcontainers.`);

  for (const productionProject of [apiProject, modulesProject, buildingBlocksProject]) {
    assertNotContains(errors, productionProject, "ContractTests", `${variant.id} production projects must not reference contract tests.`);
  }

  assertContains(errors, program, "AddAegisOpenApi", `${variant.id} pro/advanced Program.cs should wire OpenAPI contract metadata.`);
  assertContains(errors, openApiRegistration, "BearerSecuritySchemeTransformer", `${variant.id} should declare an OpenAPI bearer security scheme transformer.`);
  assertContains(errors, openApiRegistration, "AddOperationTransformer", `${variant.id} should declare protected endpoint security requirements in OpenAPI.`);
  assertContains(errors, openApiRegistration, "IAuthorizeData", `${variant.id} OpenAPI security requirements should be based on authorization metadata.`);
  assertContains(errors, openApiRegistration, "JwtBearerDefaults.AuthenticationScheme", `${variant.id} OpenAPI bearer scheme should reflect generated JWT auth.`);

  assertContains(errors, integrationEventBase, "IntegrationEventContractAttribute", `${variant.id} should include integration event contract metadata.`);
  assertContains(errors, integrationEventBase, "IntegrationEventContractMetadata", `${variant.id} should include integration event contract metadata helper.`);
  assertContains(errors, sampleContract, "IntegrationEventContract(", `${variant.id} sample integration event should declare type/version metadata.`);

  for (const [path, description] of [
    [apiContractTests, "API contract tests"],
    [integrationEventContractTests, "integration event contract tests"],
    [inboxContractTests, "inbox contract tests"]
  ]) {
    assertExists(errors, path, `${variant.id} missing ${description}.`);
    for (const token of ["Aegis.Template", "AegisProfileValue", "AegisMediatorValue", "AegisSampleValue"]) {
      assertNotContains(errors, path, token, `${variant.id} contract tests contain unresolved template token ${token}.`);
    }
  }

  for (const marker of [
    "OpenApi_document_can_be_produced_and_declares_jwt_bearer_security_scheme",
    "Expected_routes_methods_status_codes_and_content_types_are_declared",
    "Protected_endpoints_expose_named_permission_policy_metadata",
    "Permission_policy_constants_are_registered_as_named_policies",
    "Production_api_contract_does_not_reference_fake_auth_test_infrastructure",
    "AegisAuthorizationPolicies",
    "securitySchemes",
    "Bearer",
    "application/json"
  ]) {
    assertContains(errors, apiContractTests, marker, `${variant.id} API contract tests should cover ${marker}.`);
  }

  const expectedRouteMarker = variant.sample === "taskhub" ? "/tasks/" : "/work-items/";
  assertContains(errors, apiContractTests, expectedRouteMarker, `${variant.id} API contract tests should cover generated sample endpoints.`);
  if (variant.profile === "advanced") {
    assertContains(errors, apiContractTests, "/operations/advanced", `${variant.id} advanced contract tests should cover the advanced endpoint.`);
  }

  for (const marker of [
    "Integration_events_have_type_and_version_metadata_and_live_under_contracts",
    "Integration_event_contracts_round_trip_with_system_text_json",
    "Domain_events_and_integration_events_remain_distinct",
    "Inbox_handler_consumes_integration_contract_metadata_not_domain_entities",
    "System.Text.Json",
    "IntegrationEventContractAttribute",
    "IntegrationEventContractMetadata",
    ".Contracts",
    ".Domain"
  ]) {
    assertContains(errors, integrationEventContractTests, marker, `${variant.id} integration event contract tests should cover ${marker}.`);
  }

  for (const marker of [
    "Inbox_message_payload_contract_is_serializable_and_keeps_message_identity",
    "Inbox_contract_tests_do_not_require_broker_dependencies_or_exactly_once_claims",
    "MessageId",
    "IdempotencyKey",
    "IntegrationEventContractMetadata",
    "MassTransit",
    "RabbitMQ",
    "broker-level exactly-once"
  ]) {
    assertContains(errors, inboxContractTests, marker, `${variant.id} inbox contract tests should cover ${marker}.`);
  }

  assertContains(errors, docs, "## Generated Contract Tests", `${variant.id} contract docs should describe generated contract tests.`);
  assertContains(errors, docs, "not performance tests", `${variant.id} contract docs should state these are not performance tests.`);
  assertContains(errors, docs, "No broker, external identity provider, Docker runtime, or external service is required", `${variant.id} contract docs should state default independence.`);
  assertContains(errors, docs, `tests/${variant.name}.ContractTests`, `${variant.id} contract docs should name the generated contract test project.`);
}

function assertP1D3BPerformanceSmokeSemantics(errors, output, variant) {
  const solution = join(output, `${variant.name}.sln`);
  const testingDocs = join(output, "docs", "testing.md");
  const performanceDocs = join(output, "docs", "performance.md");
  const performanceRoot = join(output, "tests", `${variant.name}.PerformanceSmokeTests`);
  const performanceProject = join(performanceRoot, `${variant.name}.PerformanceSmokeTests.csproj`);
  const performanceTests = join(performanceRoot, "PerformanceSmokeTests.cs");
  const thresholds = join(performanceRoot, "Infrastructure", "PerformanceSmokeThresholds.cs");
  const assertions = join(performanceRoot, "Infrastructure", "PerformanceSmokeAssertions.cs");
  const factory = join(performanceRoot, "Infrastructure", "PerformanceSmokeWebApplicationFactory.cs");
  const authDefaults = join(performanceRoot, "Authentication", "PerformanceSmokeAuthenticationDefaults.cs");
  const authHandler = join(performanceRoot, "Authentication", "PerformanceSmokeAuthenticationHandler.cs");
  const apiProject = join(output, "src", `${variant.name}.Api`, `${variant.name}.Api.csproj`);
  const modulesProject = join(output, "src", `${variant.name}.Modules`, `${variant.name}.Modules.csproj`);
  const buildingBlocksProject = join(output, "src", `${variant.name}.BuildingBlocks`, `${variant.name}.BuildingBlocks.csproj`);

  assertExists(errors, performanceDocs, `${variant.id} should include generated performance smoke docs.`);

  if (variant.profile === "core") {
    assertMissing(errors, performanceRoot, `${variant.id} core profile should not include heavy performance smoke test assets.`);
    assertNotContains(errors, solution, "PerformanceSmokeTests", `${variant.id} core solution should not include performance smoke tests.`);
    assertContains(errors, performanceDocs, "Core does not generate the pro/advanced performance smoke test project", `${variant.id} performance docs should explain core exclusion.`);
    assertNotContains(errors, testingDocs, `tests/${variant.name}.PerformanceSmokeTests`, `${variant.id} testing docs should not name a missing performance smoke project.`);
    return;
  }

  assertExists(errors, performanceProject, `${variant.id} pro/advanced profile should include generated performance smoke tests.`);
  assertContains(errors, solution, `${variant.name}.PerformanceSmokeTests`, `${variant.id} solution should include performance smoke tests so dotnet test runs them.`);
  if (existsSync(performanceProject)) {
    const performanceProjectContent = readAbsolute(performanceProject).replaceAll("\\", "/");
    if (!performanceProjectContent.includes(`${variant.name}.Api/${variant.name}.Api.csproj`)) {
      errors.push(`${variant.id} performance smoke tests should reference the API project.`);
    }
    if (!performanceProjectContent.includes(`${variant.name}.BuildingBlocks/${variant.name}.BuildingBlocks.csproj`)) {
      errors.push(`${variant.id} performance smoke tests should reference BuildingBlocks.`);
    }
    if (!performanceProjectContent.includes(`${variant.name}.Modules/${variant.name}.Modules.csproj`)) {
      errors.push(`${variant.id} performance smoke tests should reference Modules.`);
    }
  }

  assertContains(errors, performanceProject, "Microsoft.AspNetCore.Mvc.Testing", `${variant.id} performance smoke tests should use WebApplicationFactory.`);
  assertContains(errors, performanceProject, "Microsoft.EntityFrameworkCore.InMemory", `${variant.id} performance smoke tests should override persistence with in-memory EF.`);
  assertNotContains(errors, performanceProject, "Testcontainers.PostgreSql", `${variant.id} performance smoke tests should not require Docker/Testcontainers.`);

  for (const productionProject of [apiProject, modulesProject, buildingBlocksProject]) {
    assertNotContains(errors, productionProject, "PerformanceSmokeTests", `${variant.id} production projects must not reference performance smoke tests.`);
  }

  for (const [path, description] of [
    [performanceTests, "performance smoke tests"],
    [thresholds, "performance smoke thresholds"],
    [assertions, "performance smoke assertions"],
    [factory, "performance smoke factory"],
    [authDefaults, "performance smoke fake auth defaults"],
    [authHandler, "performance smoke fake auth handler"]
  ]) {
    assertExists(errors, path, `${variant.id} missing ${description}.`);
    for (const token of ["Aegis.Template", "AegisProfileValue", "AegisMediatorValue", "AegisSampleValue"]) {
      assertNotContains(errors, path, token, `${variant.id} performance smoke assets contain unresolved template token ${token}.`);
    }
  }

  for (const marker of [
    "Api_test_host_startup_smoke_stays_within_loose_threshold",
    "Health_endpoint_response_smoke_stays_within_loose_threshold",
    "Authenticated_request_path_smoke_stays_within_loose_threshold",
    "Cqrs_dispatch_request_path_smoke_stays_within_loose_threshold",
    "OpenApi_document_generation_smoke_stays_within_loose_threshold",
    "PerformanceSmokeThresholds"
  ]) {
    assertContains(errors, performanceTests, marker, `${variant.id} performance smoke tests should cover ${marker}.`);
  }

  assertContains(errors, thresholds, "intentionally loose smoke thresholds", `${variant.id} performance smoke thresholds should document looseness.`);
  for (const marker of [
    "HostStartup",
    "SimpleRequest",
    "AuthenticatedRequest",
    "CqrsDispatchRequest",
    "OpenApiGeneration",
    "TimeSpan.FromSeconds"
  ]) {
    assertContains(errors, thresholds, marker, `${variant.id} performance smoke thresholds should include ${marker}.`);
  }

  assertContains(errors, assertions, "best warmed sample", `${variant.id} performance smoke assertions should prefer diagnostic warmed samples.`);
  assertContains(errors, assertions, "All samples", `${variant.id} performance smoke failures should report sample diagnostics.`);
  assertContains(errors, assertions, "Stopwatch", `${variant.id} performance smoke measurements should use Stopwatch.`);
  assertContains(errors, assertions, "not a benchmark or production performance certification", `${variant.id} performance smoke failures should describe diagnostic scope.`);
  assertContains(errors, factory, "UseInMemoryDatabase", `${variant.id} performance smoke tests should not require a live database.`);
  assertContains(errors, authDefaults, "Aegis.PerformanceSmoke", `${variant.id} performance smoke fake auth should be test-local.`);
  assertContains(errors, authHandler, "AegisPermissionClaimTypes.Permission", `${variant.id} performance smoke fake auth should exercise generated permission policies.`);

  const expectedPathMarker = variant.sample === "taskhub" ? "/tasks/" : "/work-items/";
  assertContains(errors, performanceTests, expectedPathMarker, `${variant.id} performance smoke tests should cover the generated CQRS request path.`);

  for (const forbidden of [
    "Testcontainers",
    "MassTransit",
    "RabbitMQ",
    "Kafka",
    "Azure.Messaging.ServiceBus",
    "IdentityServer",
    "OpenIddict",
    "Keycloak",
    "BenchmarkDotNet"
  ]) {
    assertNotContains(errors, performanceTests, forbidden, `${variant.id} performance smoke tests should not depend on ${forbidden}.`);
    assertNotContains(errors, factory, forbidden, `${variant.id} performance smoke factory should not depend on ${forbidden}.`);
  }

  assertContains(errors, testingDocs, `tests/${variant.name}.PerformanceSmokeTests`, `${variant.id} testing docs should name the generated performance smoke project.`);
  assertContains(errors, performanceDocs, "not benchmarks", `${variant.id} performance docs should state smoke tests are not benchmarks.`);
  assertContains(errors, performanceDocs, "intentionally loose", `${variant.id} performance docs should explain loose thresholds.`);
  assertContains(errors, performanceDocs, "No Docker, broker, external identity provider, external service, or real JWT issuer is required", `${variant.id} performance docs should state default independence.`);
  assertContains(errors, performanceDocs, "HostStartup", `${variant.id} performance docs should document threshold names.`);
}

function assertP1D4DeploymentSkeletonSemantics(errors, output, variant) {
  const dockerfile = join(output, "Dockerfile");
  const dockerignore = join(output, ".dockerignore");
  const compose = join(output, "docker-compose.yml");
  const envExample = join(output, ".env.example");
  const productionSettings = join(output, "src", `${variant.name}.Api`, "appsettings.Production.json");
  const deploymentDocs = join(output, "docs", "deployment.md");
  const operationsDocs = join(output, "docs", "operations.md");
  const testingDocs = join(output, "docs", "testing.md");
  const workflow = join(output, ".github", "workflows", "ci.yml");
  const apiProject = join(output, "src", `${variant.name}.Api`, `${variant.name}.Api.csproj`);
  const modulesProject = join(output, "src", `${variant.name}.Modules`, `${variant.name}.Modules.csproj`);
  const buildingBlocksProject = join(output, "src", `${variant.name}.BuildingBlocks`, `${variant.name}.BuildingBlocks.csproj`);

  assertExists(errors, deploymentDocs, `${variant.id} should include generated deployment docs.`);

  if (variant.profile === "core") {
    assertMissing(errors, dockerfile, `${variant.id} core profile should not include Dockerfile.`);
    assertMissing(errors, dockerignore, `${variant.id} core profile should not include .dockerignore.`);
    assertMissing(errors, compose, `${variant.id} core profile should not include docker-compose.yml.`);
    assertMissing(errors, envExample, `${variant.id} core profile should not include .env.example.`);
    assertMissing(errors, productionSettings, `${variant.id} core profile should not include appsettings.Production.json.`);
    assertContains(errors, deploymentDocs, "core profile keeps deployment scaffolding lightweight", `${variant.id} deployment docs should explain core exclusion.`);
    assertNotContains(errors, workflow, "docker build", `${variant.id} core CI should not include a container build job.`);
    assertNotContains(errors, workflow, "deployment-placeholder", `${variant.id} core CI should not include deployment placeholder jobs.`);
    return;
  }

  for (const [path, description] of [
    [dockerfile, "Dockerfile"],
    [dockerignore, ".dockerignore"],
    [compose, "docker-compose.yml"],
    [envExample, ".env.example"],
    [productionSettings, "appsettings.Production.json"]
  ]) {
    assertExists(errors, path, `${variant.id} pro/advanced deployment skeleton missing ${description}.`);
  }

  const normalizedDockerfile = existsSync(dockerfile) ? readAbsolute(dockerfile).replaceAll("\\", "/") : "";
  if (!normalizedDockerfile.includes(`src/${variant.name}.Api/${variant.name}.Api.csproj`)) {
    errors.push(`${variant.id} Dockerfile should restore/publish the generated API project path.`);
  }
  if (!normalizedDockerfile.includes(`src/${variant.name}.ServiceDefaults/${variant.name}.ServiceDefaults.csproj`)) {
    errors.push(`${variant.id} Dockerfile should reference the generated ServiceDefaults project path.`);
  }
  assertContains(errors, dockerfile, `${variant.name}.Api.dll`, `${variant.id} Dockerfile should run the generated API assembly.`);
  assertContains(errors, dockerfile, "mcr.microsoft.com/dotnet/sdk:10.0", `${variant.id} Dockerfile should use the .NET SDK build image.`);
  assertContains(errors, dockerfile, "mcr.microsoft.com/dotnet/aspnet:10.0", `${variant.id} Dockerfile should use the ASP.NET runtime image.`);
  assertContains(errors, dockerfile, "HEALTHCHECK", `${variant.id} Dockerfile should include a container healthcheck.`);
  assertContains(errors, dockerfile, "http://localhost:8080/health", `${variant.id} Dockerfile healthcheck should call /health.`);

  assertContains(errors, dockerignore, "**/bin/", `${variant.id} .dockerignore should exclude build outputs.`);
  assertContains(errors, dockerignore, ".env", `${variant.id} .dockerignore should exclude local env files.`);
  assertContains(errors, dockerignore, "!.env.example", `${variant.id} .dockerignore should keep the example env file.`);
  assertContains(errors, dockerignore, "tests/", `${variant.id} .dockerignore should keep runtime image context focused.`);

  for (const marker of [
    "ASPNETCORE_ENVIRONMENT=Production",
    "ConnectionStrings__Postgres=",
    "Authentication__Jwt__Issuer=",
    "Authentication__Jwt__Audience=",
    "Authentication__Jwt__SigningKey=",
    "Logging__LogLevel__Default=Information",
    "OTEL_SERVICE_NAME=",
    "OTEL_EXPORTER_OTLP_ENDPOINT=",
    "Inbox__EnableBackgroundProcessor=false",
    "Resilience__DefaultTimeoutSeconds=10",
    "Do not commit real"
  ]) {
    assertContains(errors, envExample, marker, `${variant.id} .env.example should contain ${marker}.`);
  }

  assertContains(errors, productionSettings, `"Postgres": ""`, `${variant.id} production settings should keep database connection as an empty placeholder.`);
  assertContains(errors, productionSettings, `"SigningKey": ""`, `${variant.id} production settings should keep JWT signing key empty.`);
  assertContains(errors, productionSettings, `"AllowedHosts": "example.invalid"`, `${variant.id} production settings should use a safe non-wildcard allowed-host placeholder.`);
  assertNotContains(errors, productionSettings, `"AllowedHosts": ""`, `${variant.id} production settings should not fall back to wildcard host filtering.`);
  assertNotContains(errors, productionSettings, `"AllowedHosts": "*"`, `${variant.id} production settings should not allow every host in production.`);
  assertContains(errors, productionSettings, `"EnableBackgroundProcessor": false`, `${variant.id} production settings should keep inbox processor disabled by default.`);
  assertContains(errors, productionSettings, `"OtlpEndpoint": ""`, `${variant.id} production settings should keep observability exporter empty by default.`);

  assertContains(errors, compose, "postgres:17-alpine", `${variant.id} compose should include only local PostgreSQL.`);
  assertContains(errors, compose, "Set POSTGRES_PASSWORD", `${variant.id} compose should require a supplied local PostgreSQL password.`);
  assertContains(errors, compose, "Authentication__Jwt__SigningKey: ${Authentication__Jwt__SigningKey:?Set Authentication__Jwt__SigningKey}", `${variant.id} compose should require the generated JWT signing key env var.`);
  assertNotContains(errors, compose, "${JWT_", `${variant.id} compose should align interpolation variables with .env.example.`);
  assertNotContains(errors, compose, "MassTransit", `${variant.id} compose should not include broker infrastructure.`);
  assertNotContains(errors, compose, "RabbitMQ", `${variant.id} compose should not include RabbitMQ.`);
  assertNotContains(errors, compose, "Kafka", `${variant.id} compose should not include Kafka.`);
  assertNotContains(errors, compose, "kubernetes", `${variant.id} compose should not claim Kubernetes deployment.`);
  assertNotContains(errors, compose, "Password=postgres", `${variant.id} compose should not hardcode the PostgreSQL password in connection strings.`);

  assertContains(errors, deploymentDocs, "not full production infrastructure", `${variant.id} deployment docs should scope the skeleton.`);
  assertContains(errors, deploymentDocs, "No registry, organization, repository, cloud provider, or deployment target is hardcoded", `${variant.id} deployment docs should be provider-neutral.`);
  assertContains(errors, deploymentDocs, "docker build -t", `${variant.id} deployment docs should explain container build.`);
  assertContains(errors, deploymentDocs, "docker run --rm -p 8080:8080", `${variant.id} deployment docs should explain running the image with env vars.`);
  assertContains(errors, deploymentDocs, "The generated API maps `/health`", `${variant.id} deployment docs should explain health endpoint behavior.`);
  assertContains(errors, deploymentDocs, "No collector is required by default", `${variant.id} deployment docs should explain observability does not require a backend.`);
  assertContains(errors, operationsDocs, "Deployment](deployment.md)", `${variant.id} operations docs should link deployment docs.`);
  assertContains(errors, testingDocs, "P1D-4 deployment skeleton semantics", `${variant.id} testing docs should mention P1D-4 smoke semantics.`);

  assertContains(errors, workflow, "docker build --pull -t aegis-template-api:ci .", `${variant.id} CI should include a container image build job.`);
  assertContains(errors, workflow, "deployment-placeholder", `${variant.id} CI should include a deployment placeholder job.`);
  assertContains(errors, workflow, "vars.ENABLE_DEPLOYMENT_PLACEHOLDER == 'true'", `${variant.id} deployment placeholder should be manually gated.`);
  assertContains(errors, workflow, "Add registry login, image push, and provider-specific deployment steps", `${variant.id} deployment placeholder should not deploy by default.`);
  for (const forbidden of ["REGISTRY_PASSWORD", "DOCKER_PASSWORD", "AZURE_CREDENTIALS", "AWS_ACCESS_KEY_ID", "GCP_CREDENTIALS"]) {
    assertNotContains(errors, workflow, forbidden, `${variant.id} basic generated CI should not require deployment secret ${forbidden}.`);
  }

  for (const project of [apiProject, modulesProject, buildingBlocksProject]) {
    assertNotContains(errors, project, "Dockerfile", `${variant.id} production projects should not reference Dockerfile.`);
    assertNotContains(errors, project, "docker-compose", `${variant.id} production projects should not reference compose files.`);
    assertNotContains(errors, project, ".github", `${variant.id} production projects should not reference workflow files.`);
    assertNotContains(errors, project, ".env.example", `${variant.id} production projects should not reference env examples.`);
  }

  const modulesRoot = join(output, "src", `${variant.name}.Modules`, "Modules");
  for (const moduleFile of listFilesAbsolute(modulesRoot).filter(file => file.endsWith("Module.cs"))) {
    const moduleContent = readAbsolute(moduleFile);
    if (!moduleContent.includes(`GetConnectionString("Postgres")`)) {
      continue;
    }

    if (!moduleContent.includes("string.IsNullOrWhiteSpace(connectionString)")) {
      errors.push(`${variant.id} ${relative(output, moduleFile)} should fail fast when ConnectionStrings:Postgres is blank.`);
    }
  }

  const deploymentFiles = [dockerfile, dockerignore, compose, envExample, productionSettings, deploymentDocs, workflow];
  for (const file of deploymentFiles) {
    for (const token of ["Aegis.Template", "AegisProfileValue", "AegisMediatorValue", "AegisSampleValue", "TODO_TEMPLATE"]) {
      assertNotContains(errors, file, token, `${variant.id} deployment skeleton contains unresolved template token ${token}.`);
    }
    for (const forbidden of ["super-secret", "password123", "ghcr.io/", "docker.io/", "azurecr.io", "amazonaws.com", "gcr.io"]) {
      assertNotContains(errors, file, forbidden, `${variant.id} deployment skeleton should not contain real secret/provider marker ${forbidden}.`);
    }
  }
}

function assertAiSemantics(errors, output, variant) {
  const aiFiles = [
    "AGENTS.md",
    "CLAUDE.md",
    ".github/copilot-instructions.md",
    "OpenQuestions.md",
    ".ai",
    ".agents",
    "docs/ai-development",
    "specs"
  ];

  if (variant.ai === "none") {
    for (const file of aiFiles) {
      assertMissing(errors, join(output, file), `${variant.id} ai=none should not include ${file}.`);
    }
    return;
  }

  assertExists(errors, join(output, "AGENTS.md"), `${variant.id} should include AGENTS.md.`);
  assertExists(errors, join(output, "CLAUDE.md"), `${variant.id} should include CLAUDE.md.`);
  assertContains(errors, join(output, "CLAUDE.md"), "AGENTS.md", `${variant.id} CLAUDE.md should point to AGENTS.md.`);
  assertExists(errors, join(output, ".github", "copilot-instructions.md"), `${variant.id} should include Copilot instructions because GitHub workflow output exists.`);
  assertContains(errors, join(output, ".github", "copilot-instructions.md"), "AGENTS.md", `${variant.id} Copilot instructions should point to AGENTS.md.`);
  assertExists(errors, join(output, "OpenQuestions.md"), `${variant.id} should include OpenQuestions.md.`);

  if (variant.ai === "agents") {
    for (const file of [".ai", ".agents", "specs"]) {
      assertMissing(errors, join(output, file), `${variant.id} ai=agents should not include enterprise-only ${file}.`);
    }
    return;
  }

  for (const file of [".ai/policies", ".ai/workflows", ".ai/guardrails", ".ai/evals"]) {
    assertExists(errors, join(output, file), `${variant.id} ai=enterprise should include ${file}.`);
  }

  assertExists(errors, join(output, "specs", "README.md"), `${variant.id} ai=enterprise should include specs/README.md.`);
  for (const file of ["spec.md", "plan.md", "tasks.md", "acceptance.md", "risks.md", "open-questions.md"]) {
    assertExists(errors, join(output, "specs", "_template", file), `${variant.id} ai=enterprise should include specs/_template/${file}.`);
  }
}

function assertGuardrailSemantics(errors, output, variant) {
  const runner = join(output, "tools", "guardrails", "check.mjs");
  const packageJson = join(output, "package.json");
  const ci = join(output, ".github", "workflows", "ci.yml");
  const readme = join(output, "README.md");

  if (variant.guardrails === "off") {
    assertMissing(errors, runner, `${variant.id} guardrails=off should not include the Node guardrail runner.`);
    assertMissing(errors, packageJson, `${variant.id} guardrails=off should not include guardrail package scripts.`);
    assertNotContains(errors, ci, "npm run check", `${variant.id} guardrails=off CI should not run guardrails.`);
    assertNotContains(errors, readme, "npm run check", `${variant.id} guardrails=off README should not tell users to run npm guardrails.`);
    assertNotContains(errors, join(output, ".ai", "workflows", "pre-pr-review.md"), "npm run check", `${variant.id} guardrails=off pre-PR workflow should not require npm guardrails.`);
  } else {
    assertExists(errors, runner, `${variant.id} guardrails=${variant.guardrails} should include the Node guardrail runner.`);
    assertExists(errors, packageJson, `${variant.id} guardrails=${variant.guardrails} should include package.json.`);
    for (const script of ["\"check\"", "\"check:ai\"", "\"check:docs\"", "\"check:security\"", "\"check:specs\"", "\"template:smoke\""]) {
      assertContains(errors, packageJson, script, `${variant.id} package.json should include ${script}.`);
    }
    assertContains(errors, ci, "npm run check", `${variant.id} CI should run generated guardrails.`);
    assertContains(errors, runner, "\"node_modules\"", `${variant.id} generated guardrails should ignore node_modules.`);
    assertContains(errors, runner, "\".git\"", `${variant.id} generated guardrails should ignore .git.`);
    assertContains(errors, runner, "\"bin\"", `${variant.id} generated guardrails should ignore bin.`);
    assertContains(errors, runner, "\"obj\"", `${variant.id} generated guardrails should ignore obj.`);
    assertContains(errors, runner, "appsettings.production.json", `${variant.id} generated security guardrails should scan appsettings files.`);
    assertContains(errors, runner, "password=postgres", `${variant.id} generated security guardrails should detect default PostgreSQL passwords.`);
  }

  if (variant.guardrails === "strict" && variant.ai === "enterprise") {
    assertExists(errors, join(output, ".ai", "policies", "strict-mode.md"), `${variant.id} strict enterprise output should include strict policy.`);
    assertExists(errors, join(output, ".ai", "guardrails", "strict-rules.md"), `${variant.id} strict enterprise output should include strict rules.`);
  } else {
    assertMissing(errors, join(output, ".ai", "policies", "strict-mode.md"), `${variant.id} should not include strict policy outside strict enterprise output.`);
    assertMissing(errors, join(output, ".ai", "guardrails", "strict-rules.md"), `${variant.id} should not include strict rules outside strict enterprise output.`);
  }

  if (variant.guardrails === "strict") {
    assertContains(errors, runner, "readForbiddenActions", `${variant.id} strict guardrails should parse or load forbidden actions.`);
    assertContains(errors, runner, "Blocked forbidden path detected", `${variant.id} strict guardrails should enforce blocked forbidden paths.`);
    assertContains(errors, runner, "Approval-required forbidden path detected", `${variant.id} strict guardrails should fail approval-required forbidden paths in CI.`);
  }
}

function assertHookSemantics(errors, output, variant) {
  const lefthook = join(output, "lefthook.yml");

  if (variant.hooks !== "lefthook") {
    assertMissing(errors, lefthook, `${variant.id} hooks=none variant should not include lefthook.yml.`);
    return;
  }

  assertExists(errors, lefthook, `${variant.id} hooks=lefthook variant should include lefthook.yml.`);
  if (variant.guardrails === "off") {
    assertContains(errors, lefthook, "dotnet build -c Release", `${variant.id} guardrails=off lefthook should run dotnet build.`);
    assertContains(errors, lefthook, "dotnet test -c Release --no-build", `${variant.id} guardrails=off lefthook should run dotnet test.`);
    assertNotContains(errors, lefthook, "npm run check", `${variant.id} guardrails=off lefthook should not call npm guardrails.`);
  } else {
    assertContains(errors, lefthook, "npm run check", `${variant.id} guardrails=${variant.guardrails} lefthook should run npm guardrails.`);
  }
}

function assertSkillSemantics(errors, output, variant) {
  const coreSkills = [
    "docs-writer",
    "dotnet-architecture-review",
    "dotnet-module",
    "dotnet-vertical-slice",
    "module-manifest",
    "spec-driven-feature"
  ];
  const enterpriseOnly = [
    "competitive-review",
    "efcore-migration-review",
    "guardrail-runner",
    "module-manifest-review",
    "openapi-contract-review",
    "security-review"
  ];
  const enterpriseSkills = [...coreSkills, ...enterpriseOnly];

  if (variant.ai !== "enterprise" || variant.skills === "none") {
    assertMissing(errors, join(output, ".agents", "skills"), `${variant.id} should not include .agents/skills.`);
    assertNotContains(errors, join(output, "README.md"), "Skills:", `${variant.id} README should not report generated skills when skills are not emitted.`);
    assertNotContains(errors, join(output, ".ai", "README.md"), ".agents/skills", `${variant.id} AI README should not describe removed skills.`);
    return;
  }

  const expected = variant.skills === "core" ? coreSkills : enterpriseSkills;
  for (const skill of expected) {
    const skillFile = join(output, ".agents", "skills", skill, "SKILL.md");
    assertExists(errors, skillFile, `${variant.id} skills=${variant.skills} missing ${skill}.`);

    if (existsSync(skillFile)) {
      const content = readAbsolute(skillFile);
      const referencedDocs = [...content.matchAll(/`(docs\/[^`]+\.md)`/g)].map(match => match[1]);
      for (const doc of referencedDocs) {
        assertExists(errors, join(output, doc), `${variant.id} ${skill} references missing ${doc}.`);
      }
    }
  }

  if (variant.skills === "core") {
    for (const skill of enterpriseOnly) {
      assertMissing(errors, join(output, ".agents", "skills", skill), `${variant.id} skills=core should not include enterprise-only ${skill}.`);
    }
  }
}

function assertDocsSemantics(errors, output, variant) {
  for (const file of ["docs/getting-started.md", "docs/architecture.md", "docs/development.md", "docs/testing.md", "docs/module-manifest.md", "docs/messaging.md"]) {
    assertExists(errors, join(output, file), `${variant.id} should include standard doc ${file}.`);
  }

  if (variant.docs === "standard") {
    for (const file of ["docs/security.md", "docs/operations.md", "docs/adr", "docs/ai-development"]) {
      assertMissing(errors, join(output, file), `${variant.id} docs=standard should not include expanded ${file}.`);
    }
    return;
  }

  for (const file of ["docs/security.md", "docs/operations.md", "docs/adr/0001-modular-monolith.md"]) {
    assertExists(errors, join(output, file), `${variant.id} docs=full should include ${file}.`);
  }

  if (variant.ai === "none") {
    assertMissing(errors, join(output, "docs", "ai-development"), `${variant.id} ai=none should not include AI-specific docs.`);
  } else if (variant.ai === "agents") {
    for (const file of ["agent-operating-model.md", "ai-pr-protocol.md"]) {
      assertExists(errors, join(output, "docs", "ai-development", file), `${variant.id} ai=agents docs=full should include basic AI doc ${file}.`);
    }
    for (const file of ["guardrails.md", "skills.md", "spec-driven-development.md", "workflows.md"]) {
      assertMissing(errors, join(output, "docs", "ai-development", file), `${variant.id} ai=agents should not include enterprise AI doc ${file}.`);
    }
  } else {
    for (const file of ["agent-operating-model.md", "ai-pr-protocol.md", "guardrails.md", "skills.md", "spec-driven-development.md", "workflows.md"]) {
      assertExists(errors, join(output, "docs", "ai-development", file), `${variant.id} ai=enterprise docs=full should include ${file}.`);
    }
  }
}

function assertLicenseSemantics(errors, output, variant) {
  const license = join(output, "LICENSE");
  const readme = join(output, "README.md");
  const packageJson = join(output, "package.json");

  assertExists(errors, license, `${variant.id} should include LICENSE.`);
  assertContains(errors, readme, `License: \`${variant.licenseExpression}\``, `${variant.id} README should report ${variant.licenseExpression}.`);

  if (variant.license === "apache2") {
    assertContains(errors, license, "Apache License", `${variant.id} apache2 license should contain Apache License.`);
    assertContains(errors, license, "Version 2.0", `${variant.id} apache2 license should contain Version 2.0.`);
    assertNotContains(errors, license, "MIT License", `${variant.id} apache2 license should not contain MIT License.`);
  } else {
    assertContains(errors, license, "MIT License", `${variant.id} mit license should contain MIT License.`);
    assertContains(errors, license, "Permission is hereby granted", `${variant.id} mit license should contain MIT grant text.`);
    assertNotContains(errors, license, "Apache License", `${variant.id} mit license should not contain Apache License.`);
  }

  if (existsSync(packageJson)) {
    assertContains(errors, packageJson, `"license": "${variant.licenseExpression}"`, `${variant.id} package metadata should report ${variant.licenseExpression}.`);
  }
}

function assertNoTemplateTokens(errors, output, label) {
  const tokens = [
    "AegisItemRootNamespace",
    "AegisBuildingBlocksNamespace",
    "AegisSliceModule",
    "AegisEventModule",
    "AegisWorkerModule",
    "aegis_module_schema",
    "aegis_module_owner",
    "aegis_building_blocks_project",
    "Aegis.Module",
    "Aegis.Slice",
    "Aegis.Event",
    "Aegis.Worker",
    "AegisSliceKind",
    "AegisEventScope"
  ];
  const checkedExtensions = [".cs", ".csproj", ".json", ".props", ".md"];
  const files = listFilesAbsolute(output, file => checkedExtensions.some(ext => file.endsWith(ext)));

  for (const file of files) {
    const content = readAbsolute(file);
    for (const token of tokens) {
      if (content.includes(token)) {
        errors.push(`${label} contains unresolved template token ${token} in ${file}.`);
      }
    }
  }
}

function assertNoTemplateDirectives(errors, output, label) {
  const checkedExtensions = [
    ".cs",
    ".csproj",
    ".json",
    ".props",
    ".targets",
    ".sln",
    ".md",
    ".txt",
    ".yml",
    ".yaml"
  ];
  const directivePattern = /^\s*(?:(?:\/\/|<!--)\s*)?#(?:if|else|elseif|elif|endif)\b/m;
  const csharpTemplateDirectivePattern = /^\s*#(?:if|elseif|elif)\s*\(/m;
  const files = listFilesAbsolute(output, file => checkedExtensions.some(ext => file.endsWith(ext)));

  for (const file of files) {
    const content = readAbsolute(file);
    const hasTemplateDirective =
      extname(file) === ".cs"
        ? csharpTemplateDirectivePattern.test(content)
        : directivePattern.test(content);

    if (hasTemplateDirective) {
      errors.push(`${label} contains an unresolved template conditional directive in ${file}.`);
    }
  }
}

function assertArchitectureTestSemantics(errors, output, variant) {
  const architectureRoot = join(output, "tests", `${variant.name}.ArchitectureTests`);
  const architectureProject = join(architectureRoot, `${variant.name}.ArchitectureTests.csproj`);
  const solution = join(output, `${variant.name}.sln`);
  const expectedFiles = [
    "ArchitectureTestContext.cs",
    "ModuleBoundaryTests.cs",
    "ModuleManifestTests.cs",
    "DomainIsolationTests.cs",
    "CqrsArchitectureTests.cs",
    "ContractBoundaryTests.cs",
    "ApiEndpointTests.cs",
    "InboxArchitectureTests.cs",
    "DeploymentSkeletonTests.cs",
    "PersistenceArchitectureTests.cs",
    "ProfileOptionWiringTests.cs"
  ];

  assertExists(errors, architectureProject, `${variant.id} should include generated architecture test project.`);
  assertContains(errors, solution, `${variant.name}.ArchitectureTests`, `${variant.id} solution should include architecture tests so dotnet test runs them.`);
  if (existsSync(architectureProject)) {
    const architectureProjectContent = readAbsolute(architectureProject).replaceAll("\\", "/");
    if (!architectureProjectContent.includes(`${variant.name}.BuildingBlocks/${variant.name}.BuildingBlocks.csproj`)) {
      errors.push(`${variant.id} architecture tests should reference BuildingBlocks.`);
    }
    if (!architectureProjectContent.includes(`${variant.name}.Modules/${variant.name}.Modules.csproj`)) {
      errors.push(`${variant.id} architecture tests should reference Modules.`);
    }
  }

  for (const file of expectedFiles) {
    assertExists(errors, join(architectureRoot, file), `${variant.id} architecture test suite missing ${file}.`);
  }

  const architectureFiles = listFilesAbsolute(architectureRoot, file => file.endsWith(".cs") || file.endsWith(".csproj"));
  const forbiddenTokens = [
    "Aegis.Template",
    "AegisProfileValue",
    "AegisMediatorValue",
    "AegisSampleValue",
    "AegisDatabaseValue",
    "AegisAiValue",
    "AegisGuardrailsValue",
    "AegisHooksValue",
    "AegisSkillsValue",
    "AegisDocsValue",
    "AegisLicenseValue",
    "AegisLicenseExpressionValue"
  ];
  for (const file of architectureFiles) {
    const content = readAbsolute(file);
    for (const token of forbiddenTokens) {
      if (content.includes(token)) {
        errors.push(`${variant.id} architecture tests contain unresolved template token ${token} in ${file}.`);
      }
    }
  }

  const manifestTests = join(architectureRoot, "ModuleManifestTests.cs");
  assertContains(errors, manifestTests, "allowCrossModuleDatabaseAccess", `${variant.id} architecture tests should assert manifest database boundary rules.`);
  assertContains(errors, manifestTests, "allowInfrastructureReferences", `${variant.id} architecture tests should assert manifest infrastructure-reference rules.`);
  assertContains(errors, manifestTests, "Public_contracts_listed_in_manifests_exist_under_contracts_folder", `${variant.id} architecture tests should assert manifest public contracts exist.`);
  assertContains(errors, manifestTests, "SearchOption.AllDirectories", `${variant.id} architecture tests should find public contracts recursively under Contracts.`);

  const boundaryTests = join(architectureRoot, "ModuleBoundaryTests.cs");
  assertContains(errors, boundaryTests, "Project_references_do_not_point_to_infrastructure_projects", `${variant.id} architecture tests should inspect project references for Infrastructure boundaries.`);
  assertContains(errors, boundaryTests, "Cross_module_contract_references_are_declared_in_module_manifests", `${variant.id} architecture tests should enforce manifest-declared cross-module contracts.`);

  const cqrsTests = join(architectureRoot, "CqrsArchitectureTests.cs");
  assertContains(errors, cqrsTests, "Commands_and_queries_follow_generated_abstractions", `${variant.id} architecture tests should assert CQRS request abstractions.`);
  assertContains(errors, cqrsTests, "Query_handlers_do_not_mutate_state_and_use_no_tracking_for_ef_queries", `${variant.id} architecture tests should assert query non-mutation and no-tracking EF queries.`);
  assertContains(errors, cqrsTests, "ExecuteUpdateAsync", `${variant.id} architecture tests should block EF bulk mutations from query handlers.`);
  assertContains(errors, cqrsTests, "AssertResponseTypeDoesNotExposeDomainOrInfrastructure", `${variant.id} architecture tests should prevent CQRS responses from exposing domain or infrastructure types.`);
  assertContains(errors, cqrsTests, "PublicMemberTypes", `${variant.id} architecture tests should inspect response DTO member types.`);
  assertContains(errors, cqrsTests, "AsNoTrackingWithIdentityResolution", `${variant.id} architecture tests should accept equivalent EF no-tracking APIs.`);
  assertContains(errors, cqrsTests, "IsCqrsHandlerSource", `${variant.id} architecture tests should restrict handler placement checks to CQRS handlers.`);
  assertContains(errors, cqrsTests, "MediatR.IRequest", `${variant.id} architecture tests should keep MediatR compatibility covered.`);

  const profileTests = join(architectureRoot, "ProfileOptionWiringTests.cs");
  assertContains(errors, profileTests, "AegisProfile", `${variant.id} architecture tests should read selected profile metadata.`);
  assertContains(errors, profileTests, "AegisMediator", `${variant.id} architecture tests should read selected mediator metadata.`);
  assertContains(errors, profileTests, "AddProProfileServices", `${variant.id} architecture tests should assert pro profile wiring.`);
  assertContains(errors, profileTests, "AddAdvancedProfileServices", `${variant.id} architecture tests should assert advanced profile wiring.`);
  assertContains(errors, profileTests, "services.AddMediatR", `${variant.id} architecture tests should assert MediatR wiring.`);
  assertContains(errors, profileTests, "ServiceProviderCommandDispatcher", `${variant.id} architecture tests should assert core dispatcher wiring.`);

  const domainTests = join(architectureRoot, "DomainIsolationTests.cs");
  assertContains(errors, domainTests, "Domain_source_files_do_not_depend_on_web_or_persistence_infrastructure", `${variant.id} architecture tests should assert domain isolation.`);
  assertContains(errors, domainTests, "DomainModelSourceFiles", `${variant.id} architecture tests should include domain events in domain isolation checks.`);
  assertContains(errors, domainTests, "Domain_events_are_module_owned_and_follow_the_domain_event_abstraction", `${variant.id} architecture tests should assert domain event abstraction.`);

  const endpointTests = join(architectureRoot, "ApiEndpointTests.cs");
  assertContains(errors, endpointTests, "Module_endpoint_mappings_do_not_perform_persistence_directly", `${variant.id} architecture tests should assert endpoints do not persist directly.`);
  assertContains(errors, endpointTests, "ModuleDbContextTypeNames", `${variant.id} architecture tests should reject direct module DbContext usage in endpoints.`);
  assertContains(errors, endpointTests, "*Endpoint.cs", `${variant.id} architecture tests should scan feature endpoint helper files.`);

  const persistenceTests = join(architectureRoot, "PersistenceArchitectureTests.cs");
  assertContains(errors, persistenceTests, "Each_module_has_one_module_scoped_dbcontext", `${variant.id} architecture tests should assert module-scoped DbContexts.`);
  assertContains(errors, persistenceTests, "Generated_sources_do_not_configure_cross_module_foreign_keys", `${variant.id} architecture tests should assert no cross-module FK configuration by default.`);
  assertContains(errors, persistenceTests, "relationshipMarkers", `${variant.id} architecture tests should distinguish relationship mapping from cross-module references.`);

  const inboxTests = join(architectureRoot, "InboxArchitectureTests.cs");
  assertContains(errors, inboxTests, "Inbox_profile_wiring_matches_selected_profile", `${variant.id} architecture tests should assert inbox profile wiring.`);
  assertContains(errors, inboxTests, "Domain_code_does_not_reference_inbox_infrastructure", `${variant.id} architecture tests should keep inbox infrastructure out of Domain code.`);
  assertContains(errors, inboxTests, "Inbox_processing_uses_integration_event_contracts", `${variant.id} architecture tests should assert inbox processing uses integration contracts.`);
  assertContains(errors, inboxTests, "Generated_inbox_does_not_reference_a_broker_by_default", `${variant.id} architecture tests should assert no broker dependency by default.`);

  const contractBoundaryTests = join(architectureRoot, "ContractBoundaryTests.cs");
  assertContains(errors, contractBoundaryTests, "Contract_test_project_matches_selected_profile", `${variant.id} architecture tests should assert contract-test profile behavior.`);
  assertContains(errors, contractBoundaryTests, "Production_projects_do_not_reference_contract_tests", `${variant.id} architecture tests should assert production projects do not reference contract tests.`);
  assertContains(errors, contractBoundaryTests, "Integration_contracts_do_not_depend_on_infrastructure", `${variant.id} architecture tests should assert integration contracts do not depend on infrastructure.`);
  assertContains(errors, contractBoundaryTests, "Production_api_contract_does_not_reference_fake_auth_test_infrastructure", `${variant.id} architecture tests should assert production API contracts do not reference fake auth.`);

  const deploymentTests = join(architectureRoot, "DeploymentSkeletonTests.cs");
  assertContains(errors, deploymentTests, "Deployment_skeleton_matches_selected_profile", `${variant.id} architecture tests should assert deployment profile behavior.`);
  assertContains(errors, deploymentTests, "Deployment_files_do_not_contain_hardcoded_real_secrets_or_provider_targets", `${variant.id} architecture tests should assert deployment files do not contain real secrets or provider targets.`);
  assertContains(errors, deploymentTests, "Production_projects_do_not_reference_deployment_scripts_or_workflow_files", `${variant.id} architecture tests should assert production projects do not reference deployment scripts.`);
}

function assertItemModuleSemantics(errors, moduleRoot, variant) {
  const moduleProject = join(moduleRoot, "Billing.csproj");
  const moduleClass = join(moduleRoot, "BillingModule.cs");
  const manifestPath = join(moduleRoot, "module.json");
  const dbContext = join(moduleRoot, "Infrastructure", "BillingDbContext.cs");
  const serviceRegistration = join(moduleRoot, "Infrastructure", "BillingServiceCollectionExtensions.cs");
  const domainEntity = join(moduleRoot, "Domain", "BillingEntity.cs");
  const contracts = join(moduleRoot, "Contracts", "BillingContracts.cs");
  const featuresPlaceholder = join(moduleRoot, "Features", ".gitkeep");
  const migrationsPlaceholder = join(moduleRoot, "Infrastructure", "Migrations", ".gitkeep");

  for (const [path, description] of [
    [moduleProject, "module project file"],
    [moduleClass, "module composition entry point"],
    [manifestPath, "module manifest"],
    [dbContext, "module DbContext"],
    [serviceRegistration, "module service registration extension"],
    [domainEntity, "domain folder content"],
    [contracts, "contracts folder content"],
    [featuresPlaceholder, "features folder placeholder"],
    [migrationsPlaceholder, "migrations folder placeholder"]
  ]) {
    assertExists(errors, path, `${variant.id} Billing item missing ${description}.`);
  }

  assertContains(errors, moduleProject, `<RootNamespace>${variant.name}.Modules.Modules.Billing</RootNamespace>`, `${variant.id} module item should use the generated module namespace shape.`);
  assertContains(errors, moduleProject, `${variant.name}.BuildingBlocks/${variant.name}.BuildingBlocks.csproj`, `${variant.id} module item should reference generated BuildingBlocks project.`);
  assertContains(errors, moduleClass, `namespace ${variant.name}.Modules.Modules.Billing;`, `${variant.id} module class should use generated namespace.`);
  assertContains(errors, moduleClass, "class BillingModule : IAegisModule", `${variant.id} module class should implement IAegisModule.`);
  assertContains(errors, moduleClass, "services.AddBillingInfrastructure(configuration);", `${variant.id} module class should call module service registration.`);
  assertContains(errors, moduleClass, "endpoints.MapGroup(\"/billing\")", `${variant.id} module class should map a module route group from schema.`);
  assertContains(errors, moduleClass, "MapFeatureEndpoints(group)", `${variant.id} module class should map generated feature endpoints.`);
  assertContains(errors, moduleClass, "BindingFlags.Public | BindingFlags.Static", `${variant.id} module class should discover generated slice endpoint methods.`);
  assertContains(errors, dbContext, ": DbContext", `${variant.id} module DbContext should derive from DbContext.`);
  assertContains(errors, dbContext, "DbSet<BillingEntity>", `${variant.id} module DbContext should expose a module DbSet.`);
  assertContains(errors, dbContext, "modelBuilder.HasDefaultSchema(Schema)", `${variant.id} module DbContext should set the module schema.`);
  assertContains(errors, serviceRegistration, "UseNpgsql", `${variant.id} module service registration should configure PostgreSQL.`);
  assertContains(errors, serviceRegistration, "Connection string 'Postgres' is required", `${variant.id} module service registration should fail fast without Postgres configuration.`);
  assertNotContains(errors, serviceRegistration, "Password=postgres", `${variant.id} module service registration should not include a default database password.`);

  try {
    const manifest = JSON.parse(readAbsolute(manifestPath));
    for (const property of ["name", "schema", "type", "owner", "dependencies", "publicContracts", "features", "rules"]) {
      if (!(property in manifest)) errors.push(`${variant.id} Billing module manifest missing ${property}.`);
    }
    if (manifest.name !== "Billing") errors.push(`${variant.id} Billing module manifest should record name Billing.`);
    if (manifest.schema !== "billing") errors.push(`${variant.id} Billing module manifest should record schema billing.`);
    if (manifest.type !== "business-module") errors.push(`${variant.id} Billing module manifest should be a business-module.`);
    if (manifest.rules?.allowCrossModuleDatabaseAccess !== false) errors.push(`${variant.id} Billing module manifest should disallow cross-module database access.`);
    if (manifest.rules?.allowInfrastructureReferences !== false) errors.push(`${variant.id} Billing module manifest should disallow infrastructure references.`);
  } catch (error) {
    errors.push(`${variant.id} Billing module manifest is not valid JSON: ${error.message}`);
  }
}

function assertItemSliceSemantics(errors, moduleRoot, variant) {
  const commandRoot = join(moduleRoot, "Features", "CreateInvoice");
  const queryRoot = join(moduleRoot, "Features", "ListInvoices");
  const command = join(commandRoot, "CreateInvoiceCommand.cs");
  const commandHandler = join(commandRoot, "CreateInvoiceCommandHandler.cs");
  const commandEndpoint = join(commandRoot, "CreateInvoiceCommandEndpoint.cs");
  const commandValidator = join(commandRoot, "CreateInvoiceCommandValidator.cs");
  const commandResponse = join(commandRoot, "CreateInvoiceResponse.cs");
  const query = join(queryRoot, "ListInvoicesQuery.cs");
  const queryHandler = join(queryRoot, "ListInvoicesQueryHandler.cs");
  const queryEndpoint = join(queryRoot, "ListInvoicesQueryEndpoint.cs");
  const queryResponse = join(queryRoot, "ListInvoicesResponse.cs");

  for (const path of [command, commandHandler, commandEndpoint, commandValidator, commandResponse, query, queryHandler, queryEndpoint, queryResponse]) {
    assertExists(errors, path, `${variant.id} slice item missing ${path}.`);
  }

  assertContains(errors, command, `namespace ${variant.name}.Modules.Modules.Billing.Features.CreateInvoice;`, `${variant.id} command slice should use module feature namespace.`);
  assertContains(errors, command, "ICommand<CreateInvoiceResponse>", `${variant.id} command slice should implement ICommand<TResponse>.`);
  assertContains(errors, commandHandler, "ICommandHandler<CreateInvoiceCommand, CreateInvoiceResponse>", `${variant.id} command handler should implement generated command handler contract.`);
  assertContains(errors, commandEndpoint, "ICommandDispatcher", `${variant.id} command endpoint should dispatch through ICommandDispatcher.`);
  assertContains(errors, commandEndpoint, "RouteGroupBuilder MapCreateInvoice", `${variant.id} command endpoint should expose a discoverable route group mapper.`);
  assertContains(errors, commandValidator, "IValidator<CreateInvoiceCommand>", `${variant.id} command slice should include validation convention.`);
  assertContains(errors, commandHandler, "handler tests", `${variant.id} command slice should document the test next step.`);

  assertContains(errors, query, `namespace ${variant.name}.Modules.Modules.Billing.Features.ListInvoices;`, `${variant.id} query slice should use module feature namespace.`);
  assertContains(errors, query, "IQuery<ListInvoicesResponse>", `${variant.id} query slice should implement IQuery<TResponse>.`);
  assertContains(errors, query, "PageNumber", `${variant.id} list query should include PageNumber.`);
  assertContains(errors, query, "PageSize", `${variant.id} list query should include PageSize.`);
  assertContains(errors, queryHandler, "IQueryHandler<ListInvoicesQuery, ListInvoicesResponse>", `${variant.id} query handler should implement generated query handler contract.`);
  assertContains(errors, queryEndpoint, "IQueryDispatcher", `${variant.id} query endpoint should dispatch through IQueryDispatcher.`);
  assertContains(errors, queryEndpoint, "RouteGroupBuilder MapListInvoices", `${variant.id} query endpoint should expose a discoverable route group mapper.`);
  assertContains(errors, queryEndpoint, "[AsParameters] ListInvoicesQuery query", `${variant.id} list query endpoint should bind pagination shape.`);
  assertContains(errors, queryResponse, "IReadOnlyList<ListInvoicesItemResponse>", `${variant.id} list query response should include paged item shape.`);
  assertContains(errors, queryHandler, "handler tests", `${variant.id} query slice should document the test next step.`);

  if (variant.mediator === "mediatr") {
    assertContains(errors, command, "MediatR.IRequest<CreateInvoiceResponse>", `${variant.id} MediatR command slice should implement IRequest.`);
    assertContains(errors, commandHandler, "MediatR.IRequestHandler<CreateInvoiceCommand, CreateInvoiceResponse>", `${variant.id} MediatR command handler should implement IRequestHandler.`);
    assertContains(errors, query, "MediatR.IRequest<ListInvoicesResponse>", `${variant.id} MediatR query slice should implement IRequest.`);
    assertContains(errors, queryHandler, "MediatR.IRequestHandler<ListInvoicesQuery, ListInvoicesResponse>", `${variant.id} MediatR query handler should implement IRequestHandler.`);
  } else {
    assertNotContains(errors, command, "MediatR.IRequest", `${variant.id} core command slice should not reference MediatR.`);
    assertNotContains(errors, commandHandler, "MediatR.IRequestHandler", `${variant.id} core command handler should not reference MediatR.`);
    assertNotContains(errors, query, "MediatR.IRequest", `${variant.id} core query slice should not reference MediatR.`);
    assertNotContains(errors, queryHandler, "MediatR.IRequestHandler", `${variant.id} core query handler should not reference MediatR.`);
  }
}

function assertItemEventSemantics(errors, moduleRoot, variant) {
  const domainEvent = join(moduleRoot, "Domain", "Events", "InvoiceIssuedDomainEvent.cs");
  const integrationEvent = join(moduleRoot, "Contracts", "IntegrationEvents", "InvoiceIssuedIntegrationEvent.cs");

  assertExists(errors, domainEvent, `${variant.id} domain event item should be generated under Domain/Events.`);
  assertExists(errors, integrationEvent, `${variant.id} integration event item should be generated under Contracts/IntegrationEvents.`);
  assertContains(errors, domainEvent, `namespace ${variant.name}.Modules.Modules.Billing.Domain.Events;`, `${variant.id} domain event should use domain event namespace.`);
  assertContains(errors, domainEvent, ": DomainEvent", `${variant.id} domain event should use DomainEvent abstraction.`);
  assertNotContains(errors, domainEvent, "IntegrationEvent", `${variant.id} domain event should not use IntegrationEvent abstraction.`);
  assertContains(errors, integrationEvent, `namespace ${variant.name}.Modules.Modules.Billing.Contracts.IntegrationEvents;`, `${variant.id} integration event should use integration event namespace.`);
  assertContains(errors, integrationEvent, ": IntegrationEvent", `${variant.id} integration event should use IntegrationEvent abstraction.`);
  assertNotContains(errors, integrationEvent, "DomainEvent", `${variant.id} integration event should not use DomainEvent abstraction.`);

  if (existsSync(domainEvent) && existsSync(integrationEvent) && readAbsolute(domainEvent) === readAbsolute(integrationEvent)) {
    errors.push(`${variant.id} domain and integration event outputs should be distinct.`);
  }
}

function assertItemWorkerSemantics(errors, workerRoot) {
  const worker = join(workerRoot, "BillingOutboxDispatcher.cs");
  const workerProject = join(workerRoot, "BillingOutboxDispatcher.csproj");
  const services = join(workerRoot, "BillingOutboxDispatcherServiceCollectionExtensions.cs");
  const program = join(workerRoot, "Program.cs");
  const packageProps = join(workerRoot, "Directory.Packages.props");

  assertExists(errors, worker, "worker item should generate a worker class named after the item.");
  assertExists(errors, workerProject, "worker item should generate a worker project.");
  assertExists(errors, services, "worker item should generate a DI registration extension.");
  assertMissing(errors, packageProps, "worker item should not emit Directory.Packages.props into the output folder.");
  assertContains(errors, workerProject, `PackageReference Include="Microsoft.Extensions.Hosting"`, "worker project should use the generated app central package version.");
  assertContains(errors, worker, "class BillingOutboxDispatcher", "worker item should name the BackgroundService after the requested worker.");
  assertContains(errors, worker, ": BackgroundService", "worker item should derive from BackgroundService.");
  assertContains(errors, worker, "ILogger<BillingOutboxDispatcher>", "worker item should use logging.");
  assertContains(errors, worker, "CancellationToken stoppingToken", "worker item should accept a cancellation token.");
  assertContains(errors, worker, "stoppingToken.IsCancellationRequested", "worker item should observe cancellation.");
  assertContains(errors, worker, "Task.Delay(TimeSpan.FromMinutes(5), stoppingToken)", "worker item should pass the cancellation token to delay.");
  assertContains(errors, services, "AddHostedService<BillingOutboxDispatcher>", "worker item should register as a hosted service.");
  assertContains(errors, program, "builder.Services.AddBillingOutboxDispatcher();", "worker Program.cs should use the registration extension.");
}

async function checkAi() {
  const errors = [];
  if (!existsSync(join(root, "AGENTS.md"))) errors.push("AGENTS.md is missing.");
  if (!existsSync(join(root, "OpenQuestions.md"))) errors.push("OpenQuestions.md is missing.");
  if (!existsSync(join(root, "CLAUDE.md"))) errors.push("CLAUDE.md is missing.");
  else if (!read("CLAUDE.md").includes("AGENTS.md")) errors.push("CLAUDE.md must point to AGENTS.md.");
  const copilot = ".github/copilot-instructions.md";
  if (existsSync(join(root, copilot)) && !read(copilot).includes("AGENTS.md")) {
    errors.push(`${copilot} must point to AGENTS.md.`);
  }
  if (existsSync(join(root, "AGENTS.md")) && !read("AGENTS.md").includes("specs/")) {
    errors.push("AGENTS.md must mention specs/ as the spec-driven development workspace.");
  }
  return errors.length ? fail("ai instructions", errors) : pass("ai instructions");
}

async function checkOpenQuestions() {
  const errors = [];
  const file = "OpenQuestions.md";
  if (!existsSync(join(root, file))) {
    errors.push("OpenQuestions.md is missing.");
  } else {
    const content = read(file);
    for (const section of ["## Rules", "## Status values", "## Risk values", "## Open or inferred questions", "## Blockers", "## Answered or decided"]) {
      if (!content.includes(section)) errors.push(`OpenQuestions.md missing ${section}.`);
    }
    if (!content.includes("Proposed default")) errors.push("OpenQuestions.md must require proposed defaults for entries.");
  }
  return errors.length ? fail("open questions", errors) : pass("open questions");
}

async function checkSkills() {
  const errors = [];
  const files = listFiles(".agents/skills", f => f.endsWith("SKILL.md"));
  if (files.length === 0) errors.push("No .agents/skills/**/SKILL.md files found.");
  for (const file of files) {
    const content = read(file);
    if (!content.includes("name:")) errors.push(`${file} missing name metadata.`);
    if (!content.includes("description:")) errors.push(`${file} missing description metadata.`);
    if (!content.includes("## Validation")) errors.push(`${file} missing Validation section.`);
  }
  return errors.length ? fail("skills", errors) : pass("skills");
}

async function checkWorkflows() {
  const errors = [];
  const files = listFiles(".ai/workflows", f => f.endsWith(".md"));
  if (files.length === 0) errors.push("No .ai/workflows/*.md files found.");
  for (const file of files) {
    const content = read(file);
    for (const section of ["## Purpose", "## Steps", "## Required validation", "## Human approval"]) {
      if (!content.includes(section)) errors.push(`${file} missing ${section}.`);
    }
  }
  return errors.length ? fail("workflows", errors) : pass("workflows");
}

async function checkSpecs() {
  const errors = [];
  const required = [
    "specs/README.md",
    "specs/_template/spec.md",
    "specs/_template/plan.md",
    "specs/_template/tasks.md",
    "specs/_template/acceptance.md",
    "specs/_template/risks.md",
    "specs/_template/open-questions.md",
    "specs/0001-aegis-template-core/spec.md",
    "specs/0001-aegis-template-core/plan.md",
    "specs/0001-aegis-template-core/tasks.md",
    "specs/0001-aegis-template-core/acceptance.md",
    "specs/0001-aegis-template-core/risks.md",
    "specs/0001-aegis-template-core/open-questions.md"
  ];
  for (const file of required) {
    if (!existsSync(join(root, file))) errors.push(`${file} is missing.`);
  }

  const specFiles = listFiles("specs", f => f.endsWith("spec.md"));
  if (specFiles.length === 0) errors.push("No specs/**/spec.md files found.");
  for (const file of specFiles) {
    const content = read(file);
    for (const section of ["## Status", "## Problem", "## Goals", "## Non-goals"]) {
      if (!content.includes(section)) errors.push(`${file} missing ${section}.`);
    }
  }

  const specsRoot = join(root, "specs");
  if (existsSync(specsRoot)) {
    for (const entry of readdirSync(specsRoot)) {
      const full = join(specsRoot, entry);
      if (entry === "_template" || !statSync(full).isDirectory()) continue;
      for (const name of ["spec.md", "plan.md", "tasks.md", "acceptance.md", "risks.md", "open-questions.md"]) {
        const file = `specs/${entry}/${name}`;
        if (!existsSync(join(root, file))) errors.push(`${file} is missing.`);
      }
    }
  }

  return errors.length ? fail("specs", errors) : pass("specs");
}

async function checkModuleManifestTemplate() {
  const errors = [];
  const file = "templates/module-manifest/module.json";
  if (!existsSync(join(root, file))) {
    errors.push(`${file} is missing.`);
  } else {
    try {
      const manifest = JSON.parse(read(file));
      for (const property of ["name", "schema", "type", "owner", "dependencies", "publicContracts", "features", "rules"]) {
        if (!(property in manifest)) errors.push(`${file} missing ${property}.`);
      }
      if (manifest.rules && manifest.rules.allowCrossModuleDatabaseAccess !== false) {
        errors.push(`${file} must default allowCrossModuleDatabaseAccess to false.`);
      }
      if (manifest.rules && manifest.rules.allowInfrastructureReferences !== false) {
        errors.push(`${file} must default allowInfrastructureReferences to false.`);
      }
    } catch (error) {
      errors.push(`${file} is not valid JSON: ${error.message}`);
    }
  }
  return errors.length ? fail("module manifest template", errors) : pass("module manifest template");
}

async function checkDocs() {
  const required = [
    "docs/project-brief.md",
    "docs/architecture.md",
    "docs/architecture/cqrs-lite.md",
    "docs/cli-template-spec.md",
    "docs/implementation-plan.md",
    "docs/testing.md",
    "docs/deployment.md",
    "docs/contracts.md",
    "docs/messaging.md",
    "docs/acceptance-criteria.md",
    "docs/git-plan.md",
    "docs/open-questions-protocol.md",
    "docs/open-questions-policy.md",
    "docs/competitive-analysis.md",
    "docs/getting-started/which-profile.md",
    "docs/ai-development/spec-driven-development.md",
    "docs/ai-development/ai-pr-protocol.md",
    "docs/architecture/module-manifest.md",
    "docs/architecture/vertical-slices.md",
    "docs/adr/0012-use-spec-driven-development.md"
  ];
  const errors = required.filter(f => !existsSync(join(root, f))).map(f => `${f} is missing.`);
  return errors.length ? fail("docs", errors) : pass("docs");
}

async function checkSecurity() {
  const errors = [];
  const suspicious = listFiles(".", f => {
    const normalized = f.replaceAll("\\", "/");
    return normalized.endsWith("/.env") || normalized.includes("/secrets/") || normalized.endsWith("id_rsa");
  });
  for (const file of suspicious) errors.push(`Sensitive-looking file detected: ${file}`);

  const duplicateGuardrailScripts = listFiles(".", f => {
    const normalized = f.replaceAll("\\", "/").toLowerCase();
    return (normalized.endsWith(".sh") || normalized.endsWith(".ps1")) &&
      (normalized.includes("guardrail") || normalized.includes("check"));
  });
  for (const file of duplicateGuardrailScripts) {
    errors.push(`Do not duplicate guardrail check logic in shell scripts: ${file}`);
  }

  return errors.length ? fail("security", errors) : pass("security");
}

async function checkDotnetAvailable() {
  const code = await runCommand("dotnet", ["--version"]);
  return code === 0 ? pass("dotnet available") : fail("dotnet available", ["dotnet CLI is not available."]);
}

async function checkCiWorkflows() {
  const required = [
    ".github/workflows/ci.yml",
    ".github/workflows/docs.yml",
    ".github/workflows/security.yml",
    ".github/workflows/specs.yml",
    ".github/workflows/guardrails.yml"
  ];

  const errors = required
    .filter(file => !existsSync(join(root, file)))
    .map(file => `${file} is missing.`);

  if (existsSync(join(root, ".github/workflows/ci.yml"))) {
    const ci = read(".github/workflows/ci.yml");
    for (const command of ["npm run check", "npm run template:smoke"]) {
      if (!ci.includes(command)) {
        errors.push(`.github/workflows/ci.yml must run ${command}.`);
      }
    }
  }

  return errors.length ? fail("ci workflows", errors) : pass("ci workflows");
}

async function checkTemplateSmoke() {
  const errors = [];
  const templateProject = "templates/Aegis.Modulith.Templates/Aegis.Modulith.Templates.csproj";
  if (!existsSync(join(root, templateProject))) {
    return fail("template smoke", [`${templateProject} is missing.`]);
  }

  const smokeRootBase = join(root, "artifacts", "template-smoke");
  const runsRoot = join(smokeRootBase, "runs");
  const runId = `${Date.now().toString(36)}-${randomUUID().slice(0, 8)}`;
  const smokeRoot = join(runsRoot, runId);

  mkdirSync(smokeRoot, { recursive: true });
  try {
    writeFileSync(join(smokeRootBase, "latest-run.txt"), `${smokeRoot}\n`, "utf8");
  } catch (error) {
    console.warn(`Could not update latest smoke run pointer: ${error.message}`);
  }
  console.log(`Template smoke run directory: ${smokeRoot}`);

  const packagesDir = join(smokeRoot, "p");
  const generatedRoot = join(smokeRoot, "g");
  const itemRoot = join(smokeRoot, "i");
  const dotnetHome = join(smokeRoot, "h");
  const nugetPackages = join(smokeRoot, "n");

  mkdirSync(packagesDir, { recursive: true });
  mkdirSync(generatedRoot, { recursive: true });
  mkdirSync(itemRoot, { recursive: true });
  mkdirSync(dotnetHome, { recursive: true });
  mkdirSync(nugetPackages, { recursive: true });

  let code = await runCommand("dotnet", [
    "pack",
    templateProject,
    "-c",
    "Release",
    "-o",
    packagesDir
  ]);
  if (code !== 0) {
    return fail("template smoke", ["dotnet pack failed."]);
  }

  const nupkg = readdirSync(packagesDir)
    .filter(file => file.startsWith("Aegis.Modulith.Templates.") && file.endsWith(".nupkg"))
    .sort()
    .at(-1);

  if (!nupkg) {
    return fail("template smoke", ["Template package was not produced."]);
  }

  const smokeEnv = {
    ...process.env,
    DOTNET_CLI_HOME: dotnetHome,
    DOTNET_CLI_TELEMETRY_OPTOUT: "1",
    DOTNET_NOLOGO: "1",
    NUGET_PACKAGES: nugetPackages
  };

  code = await runCommand("dotnet", [
    "new",
    "install",
    join(packagesDir, nupkg),
    "--force"
  ], { env: smokeEnv });
  if (code !== 0) {
    return fail("template smoke", ["dotnet new install failed."]);
  }

  const matrix = [
    { id: "core-core", name: "Smoke.CoreCore", profile: "core", mediator: "core", sample: "none", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "core", "--mediator", "core"] },
    { id: "core-mediatr", name: "Smoke.CoreMediatR", profile: "core", mediator: "mediatr", sample: "none", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "core", "--mediator", "mediatr"] },
    { id: "pro-core", name: "Smoke.ProCore", profile: "pro", mediator: "core", sample: "none", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "pro", "--mediator", "core"] },
    { id: "pro-mediatr", name: "Smoke.ProMediatR", profile: "pro", mediator: "mediatr", sample: "none", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "pro", "--mediator", "mediatr"] },
    { id: "advanced-core", name: "Smoke.AdvancedCore", profile: "advanced", mediator: "core", sample: "none", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "advanced", "--mediator", "core"] },
    { id: "advanced-mediatr", name: "Smoke.AdvancedMediatR", profile: "advanced", mediator: "mediatr", sample: "none", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "advanced", "--mediator", "mediatr"] },
    { id: "taskhub", name: "Aegis.TaskHub", profile: "pro", mediator: "core", sample: "taskhub", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "pro", "--sample", "taskhub"] },
    { id: "strict-enterprise", name: "Smoke.StrictEnterprise", profile: "advanced", mediator: "core", sample: "none", ai: "enterprise", guardrails: "strict", hooks: "lefthook", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "advanced", "--ai", "enterprise", "--guardrails", "strict", "--hooks", "lefthook"] },
    { id: "ai-none", name: "Smoke.AiNone", profile: "core", mediator: "core", sample: "none", ai: "none", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "core", "--ai", "none", "--guardrails", "standard", "--docs", "full"] },
    { id: "ai-agents", name: "Smoke.AiAgents", profile: "core", mediator: "core", sample: "none", ai: "agents", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "core", "--ai", "agents", "--guardrails", "standard", "--docs", "full"] },
    { id: "strict-ai-agents", name: "Smoke.StrictAiAgents", profile: "core", mediator: "core", sample: "none", ai: "agents", guardrails: "strict", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "core", "--ai", "agents", "--guardrails", "strict", "--docs", "full"] },
    { id: "guardrails-off-lefthook", name: "Smoke.GuardrailsOff", profile: "core", mediator: "core", sample: "none", ai: "enterprise", guardrails: "off", hooks: "lefthook", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "core", "--ai", "enterprise", "--guardrails", "off", "--hooks", "lefthook"] },
    { id: "skills-none-docs-standard", name: "Smoke.SkillsNoneDocsStandard", profile: "core", mediator: "core", sample: "none", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "none", docs: "standard", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "core", "--ai", "enterprise", "--skills", "none", "--docs", "standard", "--guardrails", "standard"] },
    { id: "skills-core-docs-standard", name: "Smoke.SkillsCoreDocsStandard", profile: "core", mediator: "core", sample: "none", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "core", docs: "standard", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "core", "--ai", "enterprise", "--skills", "core", "--docs", "standard", "--guardrails", "standard"] },
    { id: "skills-core-license-mit", name: "Smoke.SkillsCoreLicenseMit", profile: "core", mediator: "core", sample: "none", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "core", docs: "full", license: "mit", licenseExpression: "MIT", args: ["--profile", "core", "--ai", "enterprise", "--skills", "core", "--license", "mit", "--guardrails", "standard"] }
  ];

  for (const variant of matrix) {
    const output = join(generatedRoot, variant.id);
    code = await runCommand("dotnet", [
      "new",
      "aegis-modulith",
      "-n",
      variant.name,
      ...variant.args,
      "-o",
      output,
      "--force"
    ], { env: smokeEnv });
    if (code !== 0) {
      return fail("template smoke", [`Generation failed for ${variant.id}.`]);
    }

    const solution = join(output, `${variant.name}.sln`);
    if (!existsSync(solution)) {
      return fail("template smoke", [`${variant.id} did not generate ${variant.name}.sln.`]);
    }

    for (const command of [
      ["restore", solution],
      ["build", solution, "-c", "Release", "--no-restore"],
      ["test", solution, "-c", "Release", "--no-build"]
    ]) {
      code = await runCommand("dotnet", command, { env: smokeEnv });
      if (code !== 0) {
        return fail("template smoke", [`dotnet ${command[0]} failed for ${variant.id}.`]);
      }
    }

    assertGeneratedOptions(errors, output, variant);
    assertMediatorSemantics(errors, output, variant);
    assertProfileSemantics(errors, output, variant);
    assertP1DFeatureDepthSemantics(errors, output, variant);
    assertP1D2AAuthPermissionSemantics(errors, output, variant);
    assertP1D2BInboxSemantics(errors, output, variant);
    assertP1D3AContractTestSemantics(errors, output, variant);
    assertP1D3BPerformanceSmokeSemantics(errors, output, variant);
    assertP1D4DeploymentSkeletonSemantics(errors, output, variant);
    assertAiSemantics(errors, output, variant);
    assertGuardrailSemantics(errors, output, variant);
    assertHookSemantics(errors, output, variant);
    assertSkillSemantics(errors, output, variant);
    assertDocsSemantics(errors, output, variant);
    assertLicenseSemantics(errors, output, variant);
    assertArchitectureTestSemantics(errors, output, variant);
    assertNoTemplateDirectives(errors, output, `${variant.id} generated output`);

    if (variant.id === "taskhub") {
      for (const moduleName of ["Projects", "Tasks", "Notifications", "Audit"]) {
        const manifest = join(output, "src", `${variant.name}.Modules`, "Modules", moduleName, "module.json");
        if (!existsSync(manifest)) {
          errors.push(`TaskHub sample missing ${moduleName} module manifest.`);
        }
      }

      const starterManifest = join(output, "src", `${variant.name}.Modules`, "Modules", "WorkItems", "module.json");
      if (existsSync(starterManifest)) {
        errors.push("TaskHub sample should not include the starter WorkItems module.");
      }
    }

    if (variant.id === "core-core") {
      const starterManifest = join(output, "src", `${variant.name}.Modules`, "Modules", "WorkItems", "module.json");
      const projectManifest = join(output, "src", `${variant.name}.Modules`, "Modules", "Projects", "module.json");
      if (!existsSync(starterManifest)) {
        errors.push("Core sample-none variant should include the starter WorkItems module.");
      }
      if (existsSync(projectManifest)) {
        errors.push("Core sample-none variant should not include TaskHub Projects module.");
      }
    }

    if (variant.guardrails !== "off") {
      code = await runCommand(process.execPath, ["tools/guardrails/check.mjs", "all"], { cwd: output, env: smokeEnv });
      if (code !== 0) {
        return fail("template smoke", [`Generated guardrails failed for ${variant.id}.`]);
      }
    }
  }

  const itemCompatibilityIds = new Set(["core-core", "core-mediatr", "pro-core", "advanced-core", "pro-mediatr", "advanced-mediatr"]);
  const itemCompatibilityVariants = matrix.filter(variant => itemCompatibilityIds.has(variant.id));

  for (const variant of itemCompatibilityVariants) {
    const output = join(generatedRoot, variant.id);
    const moduleRoot = join(output, "src", `${variant.name}.Modules`, "Modules", "Billing");
    const moduleNamespace = `${variant.name}.Modules`;
    const buildingBlocksNamespace = `${variant.name}.BuildingBlocks`;
    const buildingBlocksProject = `../../../${variant.name}.BuildingBlocks/${variant.name}.BuildingBlocks.csproj`;

    code = await runCommand("dotnet", [
      "new",
      "aegis-module",
      "-n",
      "Billing",
      "--schema",
      "billing",
      "--rootNamespace",
      moduleNamespace,
      "--buildingBlocksNamespace",
      buildingBlocksNamespace,
      "--buildingBlocksProject",
      buildingBlocksProject,
      "-o",
      moduleRoot
    ], { env: smokeEnv });
    if (code !== 0) {
      return fail("template smoke", [`aegis-module generation failed for ${variant.id}.`]);
    }

    code = await runCommand("dotnet", [
      "new",
      "aegis-slice",
      "-n",
      "CreateInvoice",
      "--module",
      "Billing",
      "--kind",
      "command",
      "--mediator",
      variant.mediator,
      "--rootNamespace",
      moduleNamespace,
      "--buildingBlocksNamespace",
      buildingBlocksNamespace,
      "-o",
      moduleRoot
    ], { env: smokeEnv });
    if (code !== 0) {
      return fail("template smoke", [`aegis-slice command generation failed for ${variant.id}.`]);
    }

    code = await runCommand("dotnet", [
      "new",
      "aegis-slice",
      "-n",
      "ListInvoices",
      "--module",
      "Billing",
      "--kind",
      "query",
      "--paged",
      "true",
      "--mediator",
      variant.mediator,
      "--rootNamespace",
      moduleNamespace,
      "--buildingBlocksNamespace",
      buildingBlocksNamespace,
      "-o",
      moduleRoot
    ], { env: smokeEnv });
    if (code !== 0) {
      return fail("template smoke", [`aegis-slice query generation failed for ${variant.id}.`]);
    }

    for (const scope of ["domain", "integration"]) {
      code = await runCommand("dotnet", [
        "new",
        "aegis-event",
        "-n",
        "InvoiceIssued",
        "--module",
        "Billing",
        "--scope",
        scope,
        "--rootNamespace",
        moduleNamespace,
        "--buildingBlocksNamespace",
        buildingBlocksNamespace,
        "-o",
        moduleRoot
      ], { env: smokeEnv });
      if (code !== 0) {
        return fail("template smoke", [`aegis-event ${scope} generation failed for ${variant.id}.`]);
      }
    }

    code = await runCommand("dotnet", ["build", join(moduleRoot, "Billing.csproj"), "-c", "Release"], { env: smokeEnv });
    if (code !== 0) {
      return fail("template smoke", [`aegis-module output build failed for ${variant.id}.`]);
    }

    code = await runCommand("dotnet", ["build", join(output, `${variant.name}.sln`), "-c", "Release", "--no-restore"], { env: smokeEnv });
    if (code !== 0) {
      return fail("template smoke", [`generated solution build failed after item templates for ${variant.id}.`]);
    }

    code = await runCommand("dotnet", ["test", join(output, `${variant.name}.sln`), "-c", "Release", "--no-build"], { env: smokeEnv });
    if (code !== 0) {
      return fail("template smoke", [`generated architecture tests failed after item templates for ${variant.id}.`]);
    }

    assertItemModuleSemantics(errors, moduleRoot, variant);
    assertItemSliceSemantics(errors, moduleRoot, variant);
    assertItemEventSemantics(errors, moduleRoot, variant);
    assertNoTemplateTokens(errors, moduleRoot, `${variant.id} item template output`);
    assertNoTemplateDirectives(errors, moduleRoot, `${variant.id} item template output`);
  }

  const workerAppRoot = join(generatedRoot, "core-core");
  const workerRoot = join(workerAppRoot, "BillingOutboxDispatcher");
  code = await runCommand("dotnet", [
    "new",
    "aegis-worker",
    "-n",
    "BillingOutboxDispatcher",
    "--module",
    "Billing"
  ], { cwd: workerAppRoot, env: smokeEnv });
  if (code !== 0) {
    return fail("template smoke", ["aegis-worker generation failed."]);
  }

  code = await runCommand("dotnet", ["build", join(workerRoot, "BillingOutboxDispatcher.csproj"), "-c", "Release"], { env: smokeEnv });
  if (code !== 0) {
    return fail("template smoke", ["aegis-worker output build failed."]);
  }

  assertItemWorkerSemantics(errors, workerRoot);
  assertNoTemplateTokens(errors, workerRoot, "worker item template output");
  assertNoTemplateDirectives(errors, workerRoot, "worker item template output");

  return errors.length ? fail("template smoke", errors) : pass("template smoke");
}

const groups = {
  ai: [checkAi, checkOpenQuestions, checkSkills, checkWorkflows],
  docs: [checkDocs, checkSpecs, checkModuleManifestTemplate],
  specs: [checkSpecs, checkModuleManifestTemplate],
  manifest: [checkModuleManifestTemplate],
  manifests: [checkModuleManifestTemplate],
  security: [checkSecurity],
  dotnet: [checkDotnetAvailable],
  "template-smoke": [checkTemplateSmoke],
  all: [checkAi, checkOpenQuestions, checkSkills, checkWorkflows, checkDocs, checkSpecs, checkModuleManifestTemplate, checkCiWorkflows, checkSecurity]
};

const selected = groups[target];
if (!selected) {
  console.error(`Unknown target: ${target}`);
  process.exit(2);
}

let failed = false;
for (const check of selected) {
  const result = await check();
  if (result.ok) {
    console.log(`✓ ${result.name}`);
  } else {
    failed = true;
    console.error(`✗ ${result.name}`);
    for (const error of result.errors) console.error(`  - ${error}`);
  }
}

process.exit(failed ? 1 : 0);
