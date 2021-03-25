import {HttpClient, json} from 'aurelia-fetch-client';

export class httpClientService {
  mydata;
  
  ConfigClient(httpClient, hostURL)
  {
    httpClient.configure(config => {
      config
        .withBaseUrl(hostURL)
        .withDefaults({
          credentials: 'same-origin',
          mode: 'cors',
          headers: {
            'Accept': 'application/json',
            'X-Requested-With': 'Fetch'
            // ,
            // 'Access-Control-Allow-Origin': 'http://localhost:8080'
          }
        })
        .withInterceptor({
          request(request) {
            console.log(`Requesting ${request.method} ${request.url}`);
            return request;
          },
          response(response) {
            console.log(`Received ${response.status} ${response.url}`);
            return response;
          }
        });
    });
  }

  Get(httpClient, URI, ID)
  {

    httpClient.fetch('api/user/GetAllPersons')
      .then(response => response.json())
      .then(data => {
        console.log(data.description);
        this.mydata = data;
      });

      return this.mydata;
      
  }

  Post(httpClient, person, URI)
  {
    //'api/user/PostPerson'
    httpClient.fetch(URI, {
      method: 'post',
      body: json(person)
    })
    .then(response => response.json())
    .then(Person => {
      console.log(`Saved data! ID: ${Person.PersonID}`);
    })
    .catch(error => {
      alert('Error saving data!');
    });   
  }

  Delete(httpClient, ID)
  {
    //'4112'
    httpClient.fetch(ID, {
      method: "DELETE"
    })
      .then(response => response.json())
      .then(data => {
        console.log(data);
      });
  }

}

