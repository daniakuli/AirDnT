using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AirDnT.Data;
using AirDnT.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AirDnT.Controllers
{
    [Authorize]
    public class ApartmentsController : Controller
    {
        private readonly AirDnTContext _context;

        public ApartmentsController(AirDnTContext context)
        {
            _context = context;
        }

        // GET: Apartments
        public async Task<IActionResult> Index()
        {
            TempData["UID"] = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(await _context.Apartment.ToListAsync());
        }


        // GET: Apartments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartment
                .FirstOrDefaultAsync(m => m.ApartmentId == id);
            if (apartment == null)
            {
                return NotFound();
            }

            return View(apartment);
        }

        // GET: Apartments/Create
        [Authorize(Roles = "Admin,Owner")]
        public IActionResult Create()
        {
            if (User.IsInRole("Admin") && TempData["OwnerID"] == null)
            {
               return RedirectToAction("Index", "Owners");
            }
            return View();
        }

        // POST: Apartments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> Create([Bind("ApartmentId,DisplayName,Price,sAvailability,eAvailability,OwnerId,RoomsNumber")] Apartment apartment)
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("Admin"))
                {
                    apartment.OwnerId = (int)TempData["OwnerID"];
                }
                else
                {
                    string uname = User.Identity.Name;
                    apartment.OwnerId = _context.Owner.Where(o => o.UserName.Contains(uname)).First().OwnerId;
                }
                _context.Add(apartment);
                await _context.SaveChangesAsync();
                TempData["ApartID"] = apartment.ApartmentId;
                TempData["OwnerID"] = null;
                return RedirectToAction(nameof(Create), "ApartmentAddresses");
            }
            return View(apartment);
        }

        // GET: Apartments/Edit/5
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartment.FindAsync(id);
            if (apartment == null)
            {
                return NotFound();
            }
            return View(apartment);
        }

        // POST: Apartments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> Edit(int id, [Bind("ApartmentId,DisplayName,Price,sAvailability,eAvailability,OwnerId,RoomsNumber")] Apartment apartment)
        {
            if (id != apartment.ApartmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(apartment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApartmentExists(apartment.ApartmentId))
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
            return View(apartment);
        }

        // GET: Apartments/Delete/5
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartment
                .FirstOrDefaultAsync(m => m.ApartmentId == id);
            if (apartment == null)
            {
                return NotFound();
            }

            return View(apartment);
        }

        // POST: Apartments/Delete/5 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apartment = await _context.Apartment.FindAsync(id);
            _context.Apartment.Remove(apartment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApartmentExists(int id)
        {
            return _context.Apartment.Any(e => e.ApartmentId == id);
        }

        // Search Methods
        public async Task<IActionResult> Search(string DisplayName)
        {
            var apartments = _context.Apartment.Where(x => x.DisplayName.Contains(DisplayName));
            return Json(await apartments.ToListAsync());
        }

        

        public async Task<IActionResult> CountryAdvSearch(string Country , string City, DateTime sAvailability, DateTime eAvailability)
        {
            var apartments = from apartment in _context.Apartment
                             where apartment.Address.Country.Contains(Country) &&
                                   apartment.Address.City.Contains(City) &&
                                   apartment.sAvailability >= sAvailability &&
                                   apartment.eAvailability <= eAvailability
                             select apartment;
            return View("Index", await apartments.ToListAsync());
        }

        public async Task<IActionResult> PriceAdvSearch(int RoomsNumber, int Price, DateTime sAvailability, DateTime eAvailability)
        {
            var apartments = from apartment in _context.Apartment
                             where apartment.RoomsNumber >= RoomsNumber &&
                                   apartment.sAvailability >= sAvailability &&
                                   apartment.eAvailability <= eAvailability &&
                                   apartment.Price <= Price
                             select apartment;
            return View("Index", await apartments.ToListAsync());
        }

        public  IActionResult CountByRoomNumber()
        {
            var apartments = from apartment in _context.Apartment.AsEnumerable()
                             group apartment by apartment.RoomsNumber into g
                             select g;

            var groupedApart = apartments.Select(g => new GroupedApartment<int, Apartment>
            {
                Rooms = g.Key,
                apartments = g
            });

            return View( groupedApart.ToList());
        }
    }
}

