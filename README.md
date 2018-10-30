# Durwella.Azure.ArmTesting

Save time catching errors _locally_ in ARM templates before trying to deploy them!  
We know how it feels to wait 20 minutes for an ARM template to deploy only to find out it had some minor problem! :persevere:

# Usage

Add your ARM Templates to a `.csproj` project. Add a package reference to `Durwella.Azure.ArmTesting.Build`.  That's it!

A build target will be added to your project. That build target will search for `.json` files that smell like ARM templates.  It will then check them for basic problems (e.g. storage account name must be between 3 and 24 characters in length).  It should only take a fraction of a second.  No resources are harmed in the process!

Open an Issue to request support for a particular check.

## Limitations

- `.deployproj` projects are not yet supported. :disappointed:
- Evaluation of ARM functions is not yet supported
- Really only checking one thing right now... But you can submit an issue to request something specific!  

## References

- <https://natemcmaster.com/blog/2017/11/11/build-tools-in-nuget/>
- <https://docs.microsoft.com/en-us/azure/azure-stack/user/azure-stack-validate-templates>

