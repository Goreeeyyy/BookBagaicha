 using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using StudentManagement.Database;
using StudentManagement.Models;

namespace StudentManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentV2Controller : ControllerBase
{
    private readonly AppDbContext database;

    // Constructor of this class
    public StudentV2Controller(AppDbContext database)
    {
        this.database = database;
    }


    // Add New Student
    [HttpPost("add")]
    public IActionResult AddStudent(Student student)
    {
        database.Students.Add(student);
        database.SaveChanges();
        return Ok(student);
    }



    // Get all the Students Data
    [HttpGet("get-all")]
    [Authorize]
    public IActionResult GetAllStudents()
    {
        List<Student> allStudents = database.Students.Include(s => s.Address).ToList(); // .Include is the Eager Loading process 
        return Ok(allStudents);
    }


    // Get Student Details By ID 
    [HttpGet("get/{id}")]
    public IActionResult GetStudentById(int id)
    {
        Student? student = database.Students.FirstOrDefault(st => st.Id == id);
        return Ok(student);
    }


    // Update student of a particular id
    [HttpPut("update/{id}")]
    public IActionResult UpdateStudent(int id, Student student)
    {
        //Fetch student record from database having parameter id
        Student? st = database.Students.FirstOrDefault(st => st.Id == id);

        // make changes to st
        if (st is not null)
        {
            st.Name = student.Name;
            st.Email = student.Email;
            st.Phone = student.Phone;
            database.SaveChanges();
            return Ok(st);
        }
        return NotFound("Student of Particular ID not found");
    }

     
    // Delete existing records of Student of Particular ID
    [HttpDelete("delete")]
    public IActionResult DeleteStudent([FromQuery] int id)
    {
        int row = database.Students.Where(st => st.Id == id).ExecuteDelete();
        return Ok(row);
    }
}