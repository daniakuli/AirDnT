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
    [Authorize]
    public class ApartmentAddressesController : Controller
    {
        private readonly AirDnTContext _context;

        public ApartmentAddressesController(AirDnTContext context)
        {
            _context = context;
        }

        // GET: ApartmentAddresses
        public async Task<IActionResult> Index()
        {
            return View(await _context.ApartmentAddress.ToListAsync());
        }

        public async Task<IActionResult> Search(string searchString)
        {
            var addresses = _context.ApartmentAddress.Where(x => x.Country.Contains(searchString));
            return View("Index", await addresses.ToListAsync());
        }
        // GET: ApartmentAddresses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartmentAddress = await _context.ApartmentAddress
                .Include(a => a.Apartment)
                .FirstOrDefaultAsync(m => m.ApartmentAddressId == id);
            if (apartmentAddress == null)
            {
                return NotFound();
            }

            return View(apartmentAddress);
        }

        // GET: ApartmentAddresses/Create
        [Authorize(Roles = "Admin,Owner")]
        public IActionResult Create()
        {
            ViewData["ApartmentAddressId"] = new SelectList(_context.Apartment.Where(a => a.Address == null), "ApartmentId", nameof(Apartment.DisplayName));
            return View();
        }

        // POST: ApartmentAddresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> Create([Bind("ApartmentAddressId,Country,City,StreetName,Zip ")] ApartmentAddress apartmentAddress)
        {
            if (ModelState.IsValid)
            {
                _context.Add(apartmentAddress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index),"Apartments");
            }
            ViewData["ApartmentAddressId"] = new SelectList(_context.Apartment, "ApartmentId", "ApartmentId", apartmentAddress.ApartmentAddressId);
            return View(apartmentAddress);
        }

        // GET: ApartmentAddresses/Edit/5
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartmentAddress = await _context.ApartmentAddress.FindAsync(id);
            if (apartmentAddress == null)
            {
                return NotFound();
            }
            ViewData["ApartmentAddressId"] = new SelectList(_context.Apartment, "ApartmentId", "ApartmentId", apartmentAddress.ApartmentAddressId);
            return View(apartmentAddress);
        }

        // POST: ApartmentAddresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> Edit(int id, [Bind("ApartmentAddressId,Country,City,StreetName,Zip")] ApartmentAddress apartmentAddress)
        {
            if (id != apartmentAddress.ApartmentAddressId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(apartmentAddress);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApartmentAddressExists(apartmentAddress.ApartmentAddressId))
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
            ViewData["ApartmentAddressId"] = new SelectList(_context.Apartment, "ApartmentId", "ApartmentId", apartmentAddress.ApartmentAddressId);
            return View(apartmentAddress);
        }

        // GET: ApartmentAddresses/Delete/5
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartmentAddress = await _context.ApartmentAddress
                .Include(a => a.Apartment)
                .FirstOrDefaultAsync(m => m.ApartmentAddressId == id);
            if (apartmentAddress == null)
            {
                return NotFound();
            }

            return View(apartmentAddress);
        }

        // POST: ApartmentAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apartmentAddress = await _context.ApartmentAddress.FindAsync(id);
            _context.ApartmentAddress.Remove(apartmentAddress);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApartmentAddressExists(int id)
        {
            return _context.ApartmentAddress.Any(e => e.ApartmentAddressId == id);
        }
    }
}
