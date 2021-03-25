 import {HttpClient, json} from 'aurelia-fetch-client';
 import {ValidationRules, ValidationController, validateTrigger, ValidationControllerFactory} from 'aurelia-validation';
 import {inject, NewInstance} from 'aurelia-dependency-injection';
 import {httpClientService} from './httpClientService';

interface Todo 
{
  description: string;
  done: boolean;
}

interface Person
{
  PersonID: number;
  FirstName: string;
  LastName: string;
  MobileNumber: string;
  AddressLine1: string;
  AddressLine2: string;
  AddressLine3: string;
  PostCode: string;
  Email: string;
}

 // ValidationRules
 //   .ensure((p: Person) => p.firstName)
 //   .displayName('First Name')
 //   .required().withMessage('${$displayName} cannot be blank.');
  
@inject(NewInstance.of(ValidationController))
export class App 
{
  heading = "Todos";
  todos: Todo[] = [];
  todoDescription = '';
  firstName = '';
  lastName = '';
  email = '';
  mobileNumber = '';
  addressLine1 = '';
  addressLine2 = '';
  addressLine3 = '';
  postcode = '';
  controller = null;
  hostURL = "http://192.168.68.106/";
  personPostURI = 'api/user/PostPerson';
  personGetURI = 'api/user/GetAllPersons';
  httpClientService = new httpClientService();
  person = {
    personID: -1,
    firstName: "",
    lastName: "Kelly",
    mobileNumber: "07715950000",
    addressLine1: "Bla",
    addressLine2: "",
    addressLine3: "",
    postCode: "DJ4 8DF",
    email: "iefjeifj@aol.com"
  };
  tbody;
  htmlProperty;

  httpClient = new HttpClient();
  
  persons: Person[] = [];

  constructor(controller) {
    this.controller = controller;
    this.controller.validateTrigger = validateTrigger.blur;
    this.httpClientSetup();
    this.retrieveDataList();
  }

  httpClientSetup()
  {
    // Here we call httpClientService's ConfigClient to perform a default setup, passing in our httpClient object and our Web API's URL.
    this.httpClientService.ConfigClient(this.httpClient, this.hostURL);
  }

  submitForm()
  {
    this.person = {
      personID: -1,
      firstName: this.firstName,
      lastName: this.lastName,
      mobileNumber: this.mobileNumber,
      addressLine1: this.addressLine1,
      addressLine2: this.addressLine2,
      addressLine3: this.addressLine3,
      postCode: this.postcode,
      email: this.email
    }

    if(this.controller.errors.length <= 0)
    {
      // Here we'll persist the form data to the database. We pass in our httpClient,
      // person object, and the post URI we'll need to send our POST request to.
      this.httpClientService.Post(this.httpClient, this.person, this.personPostURI);
    }

  }

  retrieveDataList() 
  {
    this.httpClient.fetch(this.personGetURI)
      .then(response => response.json())
      .then(data => {
        this.persons = data;
        this.displayTable(this.persons);
      });

    // this.persons = this.httpClientService.Get(this.httpClient, this.personGetURI, 1);
  }

  deleteData(ID) 
  {
    this.httpClient.fetch(ID, {
      method: "DELETE"
    })
      .then(response => response.json())
      .then(data => {
        console.log(data);
      });

    // this.httpClientService.Delete(this.httpClient, this.person.PersonID);
  }

  displayTable(persons) 
  {
    for (var i = 0; i < Object.keys(persons).length; i++) 
    {
      var tr = "<tr>";

      tr += "<td>" + persons[i].PersonID + "</td>" +
        "<td>" + persons[i].FirstName + "</td>" +
        "<td>" + persons[i].LastName + "</td>" +
        "<td>" + persons[i].Email + "</td>" +
        "<td>" + persons[i].MobileNumber + "</td>" +
        "<td>" + persons[i].AddressLine1 + "</td>" +
        "<td>" + persons[i].AddressLine2 + "</td>" +
        "<td>" + persons[i].AddressLine3 + "</td>" +
        "<td>" + persons[i].PostCode + "</td>" +
        /* Here, I was trying to add buttons dynamically instead of in the HTML file.
           Unfortunately, click.trigger wasn't working, which I'm guessing is because
           the trigger isn't specified in the HTML file between the <template> tags,
           and thus Aurelia isn't able to recognise it after compilation? */
        "<td><button class='btn btn-primary' click.trigger='removeTodo(todo)'>Edit</button>" + "</td>" +
        "<td><button class='btn btn-primary' click.delegate='deleteData(1007)'>Remove</button>" + "</td>" + "</tr>";

      this.htmlProperty += tr;
    }

  }

}

ValidationRules
.ensure('firstName').required()
.withMessage('\${$displayName} cannot be blank.')
.ensure('lastName').required()
.withMessage('\${$displayName} cannot be blank.')
.ensure('email').required()
.matches(/^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/)
.ensure('mobileNumber').required()
.matches(/^(07[\d]{8,12}|447[\d]{7,11})$/)
.ensure('addressLine1').required()
.ensure('postcode').required()
.matches(/^([A-Za-z][A-Ha-hJ-Yj-y]?[0-9][A-Za-z0-9]? ?[0-9][A-Za-z]{2}|[Gg][Ii][Rr] ?0[Aa]{2})$/)
.minLength(5)
.maxLength(8)
.on(App);



