using System;
using System.Collections.Generic;

namespace Zamify.Models;

public partial class ExamLink
{
    public int Id { get; set; }

    public int ExamId { get; set; }

    public int StudentId { get; set; }

    public Guid Token { get; set; }

    public bool IsAttempted { get; set; }

    public bool IsAccessBlocked { get; set; }

    public int? TotalQuestions { get; set; }

    public int? CorrectAnswers { get; set; }

    public int? ObtainedMarks { get; set; }

    public DateTime? ExamDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
