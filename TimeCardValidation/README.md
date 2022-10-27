# HOJTimeCardValidation

## [Time Input round off]()
| **Type** | **Link** | **Changes** |
| :--- | :--- | :--- |
|Attribute|[]() (Time Round Attribute)|Create time card attribute to round of time to the nearest 15 min|
|DACExt|[TimeRoundAttribute.cs](/Attributes/TimeRoundAttribute.cs)||Time Card attribute that round of the input time to the nearest min|

## [Time Input check if holiday]()
| **Type** | **Link** | **Changes** |
| :--- | :--- | :--- |
|Screen|[EP.30.50.00]() (Time Holiday or Regular Day Validation)|check calendar if date input is holiday|
|DACExt|[TimeCardMaintExt.cs](/GraphExt/TimeCardMaintExt.cs)||Add date check on timevalidationutils that iterates to calendar execption to check if holiday or regular day is valid|