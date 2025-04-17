using System;
using System.Collections.Generic;

namespace Zamify.Models;

public partial class Question
{
    public int Id { get; set; }

    public int SubjectId { get; set; }

    public string QuestionText { get; set; } = null!;

    public string OptionA { get; set; } = null!;

    public string OptionB { get; set; } = null!;

    public string OptionC { get; set; } = null!;

    public string OptionD { get; set; } = null!;

    public string CorrectOption { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<QuestionExam> QuestionExams { get; set; } = new List<QuestionExam>();

    public virtual Subject? Subject { get; set; }// = null!;
}
