using System;
using System.Linq;
using System.Text.Json;
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
            if (User.IsInRole("Owner"))
            {

                TempData["UID"] = (from o in _context.Owner
                        where o.UserName.Contains(User.Identity.Name)
                                  select o.OwnerId).FirstOrDefault();
                
            }
            else if (User.IsInRole("Customer"))
            {
                TempData["UID"] = (from o in _context.Customer
                                  where o.UserName.Contains(User.Identity.Name)
                                  select o.Id).FirstOrDefault();
            }
            else
            {
                TempData["UID"] = 0;
            }
            //User.FindFirstValue(ClaimTypes.NameIdentifier);
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
            var apartments = _context.Apartment.Select(x => x) ;
            if(DisplayName != null)
                apartments = _context.Apartment.Where(x => x.DisplayName.Contains(DisplayName));

            foreach(var a in apartments)
            {
                a.sAvailability = DateTime.Parse(a.sAvailability.ToShortDateString());
                a.eAvailability = DateTime.Parse(a.eAvailability.ToShortDateString());
            }
            return Json(await apartments.ToListAsync());
        }

        public async Task<IActionResult> PriceAdvSearch(int RoomsNumber, double Price, DateTime sAvailability, DateTime eAvailability)
        {
            var apartments = from apartment in _context.Apartment
                             where apartment.RoomsNumber >= RoomsNumber &&
                                   apartment.sAvailability < eAvailability &&
                                   apartment.eAvailability > sAvailability &&
                                   apartment.Price <= Price
                             select apartment;
            return Json(await apartments.ToListAsync());
        }

        public async Task<IActionResult> CountryAdvSearch(string Country, string City, DateTime sAvailability, DateTime eAvailability)
        {
            var apartments = from apartment in _context.Apartment
                             where apartment.Address.Country.Contains(Country) &&
                                   apartment.Address.City.Contains(City) &&
                                   apartment.sAvailability < eAvailability &&
                                   apartment.eAvailability > sAvailability
                             select apartment;
            return Json(await apartments.ToListAsync());
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

            return View(groupedApart.ToList());
        }
        // GET Apartments/MakeRes
        public async Task<IActionResult> MakeRes(int? id)
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

        // POST Apartments/MakeRes 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeReservation(int id, [Bind("ApartmentId,DisplayName,Price,sAvailability,eAvailability,OwnerId,RoomsNumber")] Apartment apartment)
        {
            if (ModelState.IsValid)
            {
                int cusID = (from c in _context.Customer
                             where c.UserName.Contains(User.Identity.Name)
                             select c.Id).FirstOrDefault();

                Reservation res = new Reservation();
                res.ApartmentID = id;
                res.CustomerID = cusID;
                res.sAvailability = apartment.sAvailability;
                res.eAvailability = apartment.eAvailability;

                _context.Reservation.Add(res);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(apartment);
        }

        public IActionResult Stats()
        {
            return View();
        }

        public IActionResult ShowCountriesGraph()
        {
            var apartments = from address in _context.ApartmentAddress.AsEnumerable()
                             group address by address.Country into g
                             select new { key = g.Key, count = g.Count() };

            var countryCount = apartments.Select(g => new
            {
                key = g.key,
                value = g.count
            });

            TempData["graphData"] = JsonSerializer.Serialize(countryCount.ToList());

            return View("Graph");
        }

        public IActionResult ShowAvgPriceGraph()
        {
   
            var apartments = from address in _context.ApartmentAddress.AsEnumerable()
                             group address by address.Country into g
                             join apartment in _context.Apartment on g.FirstOrDefault().ApartmentAddressId equals apartment.ApartmentId
                             select new { key = g.Key, avg = g.Average(x => x.Apartment.Price) };

            var countryCount = apartments.Select(g => new
            {
                key = g.key,
                value = g.avg
            });

            TempData["graphData"] = JsonSerializer.Serialize(countryCount.ToList());

            return View("Graph");
        }
    }
}

