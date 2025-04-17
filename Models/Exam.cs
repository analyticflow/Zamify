using System;
using System.Collections.Generic;

namespace Zamify.Models;

public partial class Exam
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int SubjectId { get; set; }

    public int TeacherId { get; set; }

    public DateTime EndTime { get; set; }

    public int DurationMinutes { get; set; }

    public int TotalQuestions { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<ExamLink> ExamLinks { get; set; } = new List<ExamLink>();

    public virtual ICollection<QuestionExam> QuestionExams { get; set; } = new List<QuestionExam>();

    public virtual Subject? Subject { get; set; }// = null!;

    public virtual Teacher? Teacher { get; set; }// = null!;
}
