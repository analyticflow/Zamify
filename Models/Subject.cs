using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Zamify.Models;

public partial class Subject
{
    public int Id { get; set; }

    [Remote(action: "VerifySubject", controller: "Subjects", AdditionalFields = nameof(Id))]
    public string Name { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
