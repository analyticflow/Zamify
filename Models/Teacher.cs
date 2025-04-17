using System;
using System.Collections.Generic;

namespace Zamify.Models;

public partial class Teacher
{
    //public int Id { get; set; }
    //public string Name { get; set; } = null!;
    //public string Email { get; set; } = null!;
    //public string Password { get; set; } = null!;
    //public DateTime CreatedDate { get; set; }
    //public DateTime? UpdatedDate { get; set; }
    //public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();


    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime CreatedDate { get; set; } = DateTime.Now; // Add default value
    public DateTime? UpdatedDate { get; set; }
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

}
