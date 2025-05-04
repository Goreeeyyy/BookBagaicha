using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models; // Controllerbase is inherited from here.

namespace StudentManagement.Controllers
{
    [ApiController] // Annotations in Java/ Decorators in Python/ Attributes in C#
    // Controller is to create a backend to accept the request & 
    // handle the response for all the functionalities of Course. 

    [Route("/api/course" /*"/api/[Controller]"*/)] // Globallly declare the endpoints using Route.

    public class CourseController : ControllerBase // Controllerbase class makes it Controller class. 
    {
        static List<Course> courses = new List<Course>()
        {
            new Course()
            {
                Id = "C001",
                Name = "Computing",
                Description = "This is IT Course",
                CreditHours = 120
            },

            new Course()
            {
                Id = "C002",
                Name = "Networking",
                Description = "This is Networking IT Course",
                CreditHours = 115
            }

        };


        [HttpGet("getAll")]
        public List<Course> GetAll()
        {
            return courses;
        }


        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute(Name = "id")] string id)
        {
            Course? course = courses.FirstOrDefault(ListCourse => ListCourse.Id == id);
            return course == null ? NoContent() : Ok(course);

        }

        [HttpPost("addCourse")]
        public Course AddCourse([FromBody] Course course)
        {
            courses.Add(course);
            return course;
        }
    }
}