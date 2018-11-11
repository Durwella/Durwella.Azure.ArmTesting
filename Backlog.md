
## Backlog

- [ ] A template expression cannot exceed 24,576 characters. [[1][]]
- [ ] Template Limits [[2][]]
  - [ ] Max template size 1MB (The 1-MB limit applies to the final state of the template after it has been expanded with iterative resource definitions, and values for variables and parameters.)
  - [ ] Max parameter file size 64 KB
  - [ ] 256 parameters
  - [ ] 256 variables
  - [ ] 800 resources (including copy count)
  - [ ] 64 output values
  - [ ] 24,576 characters in a template expression


## References

- <https://github.com/marcvaneijk/arm-template-functions>

[1]: https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-functions
[2]: https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-authoring-templates#template-limits