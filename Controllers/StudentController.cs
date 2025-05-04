using Microsoft.AspNetCore.Mvc; // ControllerBase class is inherited from this package. 

// Attribute based Routing.
namespace StudentManagement.Controllers
{
    // Every C# class is not controller but every controller is C# Class.
    // This is a simple C# class.

    [ApiController] // This is an attribute (called Annotations in Java)
    [Route("api/[controller]")]

    public class StudentController : ControllerBase // This is now a controller class.
    {
        List<string> students = new() { "Bikash", "Ram", "Rita", "Shyam", "Gita" };

 
        // api/student/getAll
        [HttpGet("getAll")] // When this route link is caught from the browser.
        // [HttpGet("getStudents")]
        public /*List<string>*/ IActionResult GetAll()
        {
            // This accepts GET request from the browser. 

            return Ok(students); //Wrap in Action result
        }

        // A method can have two Links/ Route but a Route can't of two or more methods.
        // api/student
        [HttpGet("{studentId:int}")] // Path variable or Dynamic variable
        public IActionResult GetStudentById([FromRoute(Name = "studentId")] int id) // Action Method --> IActionResult interface is used to send back the status code. 
        {
            if (id > students.Count || id < 0)
            {
                return NotFound("Invalid ID Index"); //Action Result Wrapping --> Not Found Status Code
            }

            return Ok(students[id]); //Action Result Wrapping --> OK Status Code (Success)

        }
 
        // api/student?id=id
        [HttpGet("getStudent")] // FromQuery attribute is used to extract the id from user input, and compare it with id in params of below method
        public IActionResult GetStudentValueById([FromQuery] int id)
        {
            if (id > students.Count || id < 0)
            {
                return NotFound("Invalid ID Index");
            }

            return Ok(students[id]);

        }
    }

}
