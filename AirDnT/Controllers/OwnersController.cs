using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AirDnT.Data;
using AirDnT.Models;
using Microsoft.AspNetCore.Authorization;

namespace AirDnT.Controllers
{
    [Authorize(Roles = "Admin,Owner")]
    public class OwnersController : Controller
    {
        private readonly AirDnTContext _context;

        public OwnersController(AirDnTContext context)
        {
            _context = context;
        }

        // GET: Owners
        public async Task<IActionResult> Index()
        {
            return View(await _context.Owner.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search(string searchString)
        {
            var owners = _context.Owner.Where(x => x.FirstName.Contains(searchString) ||
                                                   x.LastName.Contains(searchString) ||
                                                   x.UserName.Contains(searchString));
            return View("Index", await owners.ToListAsync());
        }

        // GET: Owners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owner
                .FirstOrDefaultAsync(m => m.OwnerId == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // GET: Owners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Owners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OwnerId,FirstName,LastName,PhoneNum,Email")] Owner owner)
        {
            if (ModelState.IsValid)
            {
                owner.UserName = (string)TempData["UserName"];
                _context.Add(owner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(owner);
        }

        public IActionResult AddApartment(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                if (User.IsInRole("Admin"))
                {
                    TempData["OwnerID"] = id;
                }
                return RedirectToAction(nameof(Create), "Apartments");
            }

            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShowOwnerApart()
        {
            var apartments = from o in _context.Owner
                             join a in _context.Apartment on o.OwnerId equals a.OwnerId
                             select new OwnerApartments
                             {
                                 DisplayName = a.DisplayName,
                                 FirstName = o.FirstName,
                                 LastName = o.LastName
                          };
            return View(await apartments.ToListAsync());
        }

        public async Task<IActionResult> ShowApartmentByCountry(string country, int id)
        {
            var apartments = from a in _context.Apartment
                             join add in _context.ApartmentAddress on a.ApartmentId equals add.Apartment.ApartmentId
                             join o in _context.Owner on a.OwnerId equals o.OwnerId
                             where add.Country == country && o.OwnerId == id
                             select a;

            return View(await apartments.ToListAsync());
        }

        // GET: Owners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owner.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }
            return View(owner);
        }

        // POST: Owners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OwnerId,FirstName,LastName,PhoneNum,Email")] Owner owner)
        {
            if (id != owner.OwnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(owner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnerExists(owner.OwnerId))
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
            return View(owner);
        }

        // GET: Owners/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owner
                .FirstOrDefaultAsync(m => m.OwnerId == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // POST: Owners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var owner = await _context.Owner.FindAsync(id);
            _context.Owner.Remove(owner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OwnerExists(int id)
        {
            return _context.Owner.Any(e => e.OwnerId == id);
        }

        
    }
}
