# JobTest
Programming Assignment

The assignment was completed in C# and targeted for .Net 5. Data Storage was accomplished with Microsoft SQL Server Express. As for the front-end technology, I used combination of Razor and React JS.  

Since a vehicle must be assigned to a category, a one-to-many relationship must be utilized. That means one vehicle can have one category, but one category can have many vehicles. I have utilised code first approach in Entity Framework Core to achieve this. Careful design is required to achieve the one-to-many approach and to prevent cascade delete (in case of one category is deleted we do not want to delete the corresponding vehicles).

I have created requirements regarding the fields of the Vehicle’s class where all the fields are required, and the weight field cannot be less than zero and more than 10000. 

When the application runs, it goes to the vehicles list in which user can create, edit, view, and delete a certain vehicle. User also can sort vehicles based on the owner’s name, manufacturer, year of manufacture, and weight of the vehicle. I have decided to accomplish this with MVC as it is easy to do CRUD with MVC.  When a new vehicle is created, the application assigns it to a category. This is done in the VehicleController where I created a function to fetch a category based on the vehicle’s weight and utilize the navigation property in the Vehicle’s class to make it point to the fetched category. Same process will take place when the weight of a vehicle is edited, the new category will be fetched and assigned to the vehicle. 

A private function has been created in the VehicleController to fetch an image of the corresponding category as URI. The function takes the id of the Vehicle and returns IActionResult where it can be assigned immediately to the “src” attribute of “img” tag. 

As for the Categories, MVC is utilized to view only the list of the available categories. Much of the functionality related to the categories is done using the “CategoriesApiController” as a back-end API and React JS for the front-end. React JS displays the categories in cards, each category in a card, with buttons and edit and delete. The categories are sorted based on their minimum value. 

Since working with the categories requires many constraints, such as no gap, no overlap, etc., I have created the following steps with simple algorithms to avoid gaps and overlaps when add, delete, and edit. 

•	Add new category: 
When adding a new category, range adjustment must be done to avoid any overlap in the range and eliminate any gap among the ranges of the categories. If a new category is added, first a list of categories is fetched and sorted based on their minimum value. The new category will be added at the end of the list and it will occupy the half range of the last category and the last category range will shrink by half. So, no overlap or gap, and if the user does not like the new ranges, they can change it as will be explained later. The following steps must be followed: 
1.	Adjust ranges of the existing categories by shrinking the range of the last category to the half. 
2.	Add the new category to the end of the list. 
3.	Update vehicles category based on the new categories’ ranges. 
This is all done in the back-end and the front-end. In the front-end, I have created a button to add which is a toggle button when pressed a drop-down will shown with the form for adding new category, and its text changes to “cancel” and colour changes to red and when clicked again, the form disappears. In this form users cannot add a range for the created category to avoid gaps and overlaps as explained. 

•	Edit an existing category: 
To edit an existing category, a list of categories is shown where each category is shown in a box with two buttons, “Edit”, and “Cancel”. At first, “Cancel” is inactive only the “Edit”, it becomes active when “Edit” clicked, where the “Edit” changes text to “Save Changes”. Nothing fancy is going on here as this functionality will not change the ranges of the categories. 

•	Edit categories ranges: 
Users can view the edit range form by clicking on the toggle button “Edit Categories Ranges”. To avoid any gaps and overlaps, and for better user experience, I have used the React slider component from https://codesandbox.io/s/k91omlr1wo.  This component has one limitation, users can change the start and end of the ranges. However, we can check for that easily in JavaScript and alert users if this happens. The component takes the ranges as an array and sends back the selected ranges in an array which is sent to the web API for back-end processing. These following steps must be taken in the back-end to avoid gaps and overlaps: 
1.	The categories ranges are updated category by category. 
2.	Vehicles’ categories are updated. 

•	Delete existing category: 
To delete, a red cross sign is placed at the top-right of each category box. When a user clicks on it, delete function is invoked at the back-end. To avoid gaps and overlaps, we devised an algorithm which is based on three scenarios as follows:
1.	If the deleted category is the first in the list, the second category will occupy the deleted category range.
2.	If the deleted category is in the middle of two categories, the neighbouring categories will take half each from the range of the deleted category. 
3.	If the deleted category is the last in the list, the category before the deleted category will occupy the deleted category range. 
The following steps are performed at the back-end: 
1.	Fetch all the vehicle under the deleted category. 
2.	Adjust the category ranges based on the above. 
3.	Delete the category. 
4.	Assign the vehicles fetched in (1) to other categories based on their weight and the newly updated ranges. 
5.	Update all the vehicles ranges. 

Finally, the React JS application is under the folder “ClientApp” under the project main folder. 
