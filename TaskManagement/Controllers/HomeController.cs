using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TaskManagement.Data;
using TaskManagement.Interface;
using TaskManagement.Models;
using TaskManagement.ViewModel;

namespace TaskManagement.Controllers
{
    public class HomeController : Controller
    {

        private readonly DataContext _ctx;
        private readonly IProgrammer _programmer;

        public HomeController(DataContext ctx, IProgrammer programmer)
        {
            _ctx = ctx;
            _programmer = programmer;
        }

        public IActionResult Index()
        {
            ICollection<Programmer> programmers = _programmer.GetAll();
            return View(programmers);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var details = await _programmer.GetByIDAsync(id);
            if (details == null) return View("Error");
            return View(details);
        }
        [HttpPost, ActionName("DeleteProgrammer")]
        public async Task<IActionResult> DeleteProgrammer(int id)
        {
            var details = await _programmer.GetByIDAsync(id);
            if (details == null) return View("Error");
            _programmer.Delete(details);
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            var unassignedAssignments = _ctx.Assignments.Where(a => a.ProgrammerId == null).ToList();

            ViewBag.UnassignedAssignments = new SelectList(unassignedAssignments, "id", "name");

            return View();
        }

        [HttpPost, ActionName("Create")]
        public IActionResult Create(Programmer programmer, int? selectedAssignmentId)
        {
            if (ModelState.IsValid)
            {
                // Assign the selected assignment to the programmer, if necessary
                if (selectedAssignmentId != 0)
                {
                    var assignment = _ctx.Assignments.FirstOrDefault(a => a.id == selectedAssignmentId);
                    if (assignment != null)
                    {
                        if (programmer.assignments == null)
                            programmer.assignments = new List<Assignment>();

                        programmer.assignments.Add(assignment);
                        assignment.ProgrammerId = programmer.id; // Link the assignment to the new programmer
                    }
                }

                // Add the programmer (with the assignment) to the context and save
                _ctx.Add(programmer);
                _ctx.SaveChanges();

                return RedirectToAction("Index");
            }

            // If we got this far, something failed; re-display form
            ViewBag.UnassignedAssignments = new SelectList(_ctx.Assignments.Where(a => a.ProgrammerId == null), "id", "name", selectedAssignmentId);
            return View(programmer);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var programmer = await _ctx.Programmers
                .Include(p => p.assignments)
                .FirstOrDefaultAsync(p => p.id == id);

            var allAssignments = await _ctx.Assignments.ToListAsync();

            var viewModel = new UpdateProgrammerViewModel
            {
                Programmer = programmer,
                AvailableAssignments = allAssignments.Select(a => new SelectListItem
                {
                    Value = a.id.ToString(),
                    Text = a.name,
                    Selected = programmer.assignments.Any(pa => pa.id == a.id)
                }).ToList(),
                SelectedAssignments = programmer.assignments.Select(a => a.id).ToList()
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> Edit(int id, UpdateProgrammerViewModel model)
        {
            if (id != model.Programmer.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // The 'Update' method now also takes care of assignment updates.
                    bool updateSucceeded = _programmer.Update(model.Programmer, model.SelectedAssignments);

                    if (!updateSucceeded)
                    {
                        return View("Error"); // Or handle the error as you see fit
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_programmer.ProgrammerExists(model.Programmer.id))
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
            // If we got this far, something failed, redisplay form
            return View(model);
        }

    }

}
