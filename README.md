## Synopsis

Evenue is an application which allow us to manage and look for events around us. Developed using Visual Studio 2015, Evenue shows us some powerful features of the Universal Windows Platform and Microsoft Azure.

## Installation

Fork or download this project and open the solution using Visual Studio 2015. Make sure you are in the developer mode in Windows 10 to be able to run the application.

## Tests

Follow these steps to be able to run the application :
  1. In Evenue.ClientApp, open app.xaml.cs and change the following template with the real data from your Azure Mobile App Service:
  
  ```cs
  public static MobileServiceClient MobileService =
    new MobileServiceClient(
      "[Mobile App URL]",
      "[Gateway URL]",
      ""
    );
  ```
  2. In Evenue.MobileAppService, change the following template with the real data from your Azure Mobile App Service and Azure Storage Account:
  
  ```xml
  <add key="MS_MobileServiceName" value="[Mobile Service Name]" />
  <add key="MS_SigningKey" value="Overridden by portal settings" />
  <add key="EMA_RuntimeUrl" value="[Gateway URL]" />
  <add key="STORAGE_ACCOUNT_NAME" value="[Storage Account Name]" />
  <add key="STORAGE_ACCOUNT_ACCESS_KEY" value="[Storage Account Access Key]" />
  ```
  
  3. Publish the Evenue.MobileAppService project to your Azure Mobile App Service profile
  
  4. Run the application and see what it can do for you!

