using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Task4.Implementation.ExamSystem;

namespace Task4.WebAPI.Controllers;

[ApiController]
[Route("/api/[controller]/[action]")]
public class ExamSystemController : ControllerBase
{
    private readonly ILogger<ExamSystemController> _logger;
    private readonly IExamSystem _examSystem;

    public ExamSystemController(ILogger<ExamSystemController> logger, IExamSystem examSystem)
    {
        _logger = logger;
        _examSystem = examSystem;
    }
    
    [HttpGet(Name = "GetCount")]
    public int Count()
    {
        return _examSystem.Count;
    }
    
    [HttpGet(Name = "GetContains")]
    public bool Contains([Required] long studentId, [Required] long courseId)
    {
        _logger.LogInformation("Params: studentId={StudentId}, courseId={CourseId}", studentId, courseId);
        return _examSystem.Contains(studentId, courseId);
    }
    
    [HttpPost(Name = "AddCredit")]
    public void Add([Required] long studentId, [Required] long courseId)
    {
        _logger.LogInformation("Params: studentId={StudentId}, courseId={CourseId}", studentId, courseId);
        _examSystem.Add(studentId, courseId);
    }
    
    [HttpDelete(Name = "RemoveCredit")]
    public void Remove([Required] long studentId, [Required] long courseId)
    {
        _logger.LogInformation("Params: studentId={StudentId}, courseId={CourseId}", studentId, courseId);
        _examSystem.Remove(studentId, courseId);
    }
}