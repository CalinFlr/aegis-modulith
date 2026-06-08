# Guardrails

Generated guardrails mode: `AegisGuardrailsValue`.

#if (guardrails == "off")
Guardrails are disabled. The generated app does not include the Node guardrail runner or guardrail npm scripts.
#endif
#if (guardrails == "standard")
Standard mode includes deterministic checks for AI instruction pointers, docs shape, security-sensitive files, specs, skills, and workflows when those assets are generated.
#endif
#if (guardrails == "strict")
Strict mode includes standard checks plus stricter policy files, strict guardrail rules, sensitive-file checks, AI instruction consistency checks, and shape checks for skills, workflows, and specs.
#endif
