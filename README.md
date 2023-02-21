# REST-API client example code in .NET6

This is a simple example of a REST-API client developed in.NET6, users can use this code as a boilerplate to integrate with Ayyeka's FAI platform, example code only works with version 2.0 of Rest-API.

When running the code the following steps will be executed:
1. Authentication using a specified API Key, Secret in order to acquire an access token.
2. Fetch and print out metadata (sites, samples).
3. A loop that fetches new samples from the FAI Rest-API every 15min.

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
* We used NSwagStudio to generate the API client in c#.
* We used openapi3.json openapi3.0 spec file Ayyek REST-API


## How to re-generate AyyekaApiClient using NSwagStudio
We recommend using the NSwagStudio and not generating directly from VS2022 because the default configuration of NSwag generating code can't be compiled "right out of the box", here are the steps to regenerate the API-client from openapi3 spec.

1. Download and install NSwagStudio from https://github.com/RicoSuter/NSwag/wiki/NSwagStudio.
2. Download the latest openapi3 spec from https://developer.ayyeka.com/docs/api-clients.
3. Open NSwagStudio and paste openapi3.json (or yaml).
4. Goto the CSharp Client tab in the Outputs, then under the Settings tab update the following settings:
    4.1 Set _Namespace_ textbox to be Ayyeka
    4.2 Under Client option - validate that _Generate Client Classes_ is checked and change the _OperationGeneration Mode_ to _SingleClientFromPathSegments_
    4.3 Change other settings if it is required.
    4.4 Click _Generate Outputs_ and copy the created csharp code to AyyekaApiClient.cs
