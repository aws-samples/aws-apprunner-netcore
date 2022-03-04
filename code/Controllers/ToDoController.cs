/*
 * Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * SPDX-License-Identifier: MIT-0
 */
 
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;

namespace todo_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToDoController : ControllerBase
    {

        private IOptions<Parameters> _options;
        private readonly ILogger<ToDoController> _logger;

        public ToDoController(IOptions<Parameters> options, ILogger<ToDoController> logger)
        {            
            _options = options;
            _logger = logger;
            _logger.LogInformation("ToDo Controller!");
        }
        
        // GET api/todo/<status>
        [HttpGet("{status}")]
        public ActionResult<string> Get(String status)
        {
            String data = "";
            _logger.LogInformation("getting status - " + status);
             using (var db = new ToDoContext(_options))
                {
                   var todos = from t in db.ToDos
                           where t.Status == status
                           select t;
                   
                   foreach(ToDo t in todos){
                       data += t.Id + " : " + t.Task + " : " + t.Status + " : " + t.CreatedTime + "\n";                   
                   }
                }
            _logger.LogInformation("Data - " + data);
            return data;
        }

        // POST api/todo
        [HttpPost]
        public void Post(ToDo todoVal)
        {
            _logger.LogInformation("Entering ToDo Put - " + JsonConvert.SerializeObject(todoVal, Formatting.Indented));

            if (todoVal != null && !string.IsNullOrEmpty(todoVal.Status) &&
                    !string.IsNullOrEmpty(todoVal.Task))
            {
                _logger.LogInformation("Saving DBContext!");
                try
                {
                    using (var db = new ToDoContext(_options))
                    {
                        db.Database.EnsureCreated();

                        todoVal.CreatedTime = DateTime.Parse(DateTime.Now.ToString(new CultureInfo("en-US" )));
                        var todo = db.ToDos.Add(todoVal).Entity;
                        db.SaveChanges();                        
                    }
                }
                catch (System.Exception ex)
                {
                    _logger.LogInformation("Error occurred - " + ex.GetBaseException().Message);
                }
                
            }

            _logger.LogInformation("ToDo Saved Succesfully!");
        }
    }
}
