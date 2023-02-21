# REST-API client example code in .NET6

This is a simple example of REST-API client developed in .NET6,user can use this code as boilareplate for integration with Ayyeka's FAI platform.

Example code will authenticate and fetch metadata and will fetch in a loop for new samples from the FAI platform.

## Prerequisites

### API Key and Secret
* Follow the instructions at [Ayyeka Developer Portal](https://developer.ayyeka.com/docs/authentication) to generate the API Key,Secret.
* Update the Program.cs with generated the API Key,Secret.

```
    private const string API_KEY = "YOUR_API_KEY";
    private const string API_SECRET = "YOUR_API_SECRET";

```
    

### Developer Environment & Tools

* We used VS2022 with .net6.
* We used NSwagStudio to generate the apiclient in c#, it recommeded use the NSwagStudio and not generate directly from VS2022 since the default configuration of NSwag generating code that can't compiled, see section Below.
* We used openapi3.json openapi3.0 spec file Ayyek REST-API


## How to re-generate AyyekaApiClient using NSwagStudio
1.Download and install NSwagStudio from https://github.com/RicoSuter/NSwag/wiki/NSwagStudio.
2.Download the the latest openapi3 spec from https://developer.ayyeka.com/docs/api-clients.
3.Open NSwagStudio and paste openapi3.json (or yaml).
4.Goto the CSharp Client tab in the Outputs,than under Settings tab update the following settings:
    4.1 Set _Namespace_ textbox to be Ayyeka
    4.2 Under Client option - validate that _Generate Client Classes_ is checked and change the _OperationGeneration Mode_ to _SingleClientFromPathSegments_
    4.3 Change other settings if it is required.
    4.4 Click _Generate Outputs_ and copy the created csharp code to AyyekaApiClient.cs


