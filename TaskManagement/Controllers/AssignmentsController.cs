using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Data.Enum;
using TaskManagement.Interface;
using TaskManagement.Models;
using TaskManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Controllers
{
    public class AssignmentsController : Controller
    {
        private readonly DataContext _ctx;
        private readonly IAssignment _assignment;
        public AssignmentsController(DataContext ctx, IAssignment assignment)
        {
            _ctx = ctx;
            _assignment = assignment;
        }
        public ActionResult Index()
        {
            ICollection<Assignment> assignments = _assignment.GetAll();
            return View(assignments);
        }

        public IActionResult Create()
        {
            ViewBag.AvailableParents = new SelectList(_ctx.Assignments.Where(a => a.state != State.Closed)
            .Select(a => new { a.id, a.name }), "id", "name");

            ViewBag.AvailableProgrammers = new SelectList(_ctx.Programmers.Where(p => !p.assignments.Any() || p.assignments.All(a => a.state == State.Closed))
            .Select(p => new { p.id, FullName = p.fname + " " + p.lname }), "id", "FullName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Create")]
        public IActionResult Create(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                assignment.state = State.New; // Set the state to New
                assignment.startDate = DateTime.Now; // Set the start date to now

                _assignment.Add(assignment);


                return RedirectToAction(nameof(Index));
            }

            // If we get here, something went wrong, repopulate the ViewBag and show the form again
            ViewBag.AvailableParents = new SelectList(_ctx.Assignments.Where(a => a.state != State.Closed)
            .Select(a => new { a.id, a.name }), "id", "name");
            ViewBag.AvailableProgrammers = new SelectList(_ctx.Programmers.Where(p => !p.assignments.Any() || p.assignments.All(a => a.state == State.Closed))
            .Select(p => new { p.id, FullName = p.fname + " " + p.lname }), "id", "FullName");

            return View(assignment);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var details = await _assignment.GetByIDAsync(id);
            if (details == null) return View("Error");
            return View(details);
        }
        [HttpPost, ActionName("DeleteAssignment")]
        public async Task<IActionResult> DeleteProgrammer(int id)
        {
            var details = await _assignment.GetByIDAsync(id);
            if (details == null) return View("Error");
            _assignment.Delete(details);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var assignment = _ctx.Assignments.Find(id);
            if (assignment == null)
            {
                return NotFound();
            }

            // Create a SelectList with "None" option
            var noneOption = new[] { new { id = (int?)null, name = "None" } };
            var parentAssignments = _ctx.Assignments
                                         .Where(a => a.state != State.Closed && a.id != id)
                                         .Select(a => new { id = (int?)a.id, name = a.name })
                                         .ToList();

            // Combine "None" option with other assignments
            var parentAssignmentsSelectList = noneOption.Concat(parentAssignments.Select(a => new { id = a.id, name = a.name }))
                                                         .ToList();

            ViewData["ParentAssignments"] = new SelectList(parentAssignmentsSelectList, "id", "name", assignment.ParentId);
            ViewData["Programmers"] = new SelectList(_ctx.Programmers, "id", "fname", assignment.ProgrammerId);

            return View(assignment);
        }
        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> Edit(int id, Assignment assignment)
        {

            if (id != assignment.id)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                try
                {
                    bool updateResult = _assignment.Update(assignment);
                    if (!updateResult)
                    {
                        return NotFound(); // Or handle the failure as appropriate
                    }
                    await _ctx.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_assignment.AssignmentExists(assignment.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // Repopulate ViewData if we're returning to the form due to validation failure
            ViewData["ParentId"] = new SelectList(_ctx.Assignments.Where(a => a.state != State.Closed), "id", "name", assignment.ParentId);
            ViewData["ProgrammerId"] = new SelectList(_ctx.Programmers, "id", "fname", assignment.ProgrammerId);

            return View(assignment);
        }

    }

}
