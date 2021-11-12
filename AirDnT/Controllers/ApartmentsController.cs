using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AirDnT.Data;
using AirDnT.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.IO;

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

            var apartment = await _context.Apartment.ToListAsync();
            var apartmentAddress = await _context.ApartmentAddress.ToListAsync();

            for(var i = 0; i < apartment.Count(); i++)
            {
                apartment[i].Address = apartmentAddress[i];
            }


            return View(apartment);
        }

        // GET: Apartments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartment.FirstOrDefaultAsync(a => a.ApartmentId == id);
            if (apartment == null)
            {
                return NotFound();
            }
            var apartmentAddress = await _context.ApartmentAddress.FirstOrDefaultAsync(ad => ad.ApartmentAddressId == id);
            apartment.Address = apartmentAddress;

            
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

            var apartment = await _context.Apartment.FirstOrDefaultAsync(m => m.ApartmentId == id);
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
            var apartments = _context.Apartment.Join(_context.ApartmentAddress,
                                                           a => a.ApartmentId,
                                                           ad => ad.ApartmentAddressId,
                                                           (a, ad) => new { Apartment = a, ApartmentAddress = ad })
                                                     .Select(a => new
                                                     {
                                                         Apartment = a.Apartment,
                                                         country = a.ApartmentAddress.Country,
                                                         City = a.ApartmentAddress.City
                                                     });
            if(DisplayName != null)
            {
                apartments = apartments.Where(a => a.Apartment.DisplayName.Contains(DisplayName));
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
                             where apartment.Address.Country.Contains(Country) && apartment.Address.City.Contains(City) 
                                   && apartment.sAvailability < eAvailability && apartment.eAvailability > sAvailability
                             select apartment;
            return Json(await apartments.ToListAsync());
        }

        // GET Apartments/MakeRes
        public async Task<IActionResult> MakeRes(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartment.FirstOrDefaultAsync(m => m.ApartmentId == id);
            if (apartment == null)
            {
                return NotFound();
            }
            return View(apartment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeReservation([Bind("ApartmentId,DisplayName,Price,sAvailability,eAvailability,OwnerId,RoomsNumber")] Apartment apartment)
        {
            if (ModelState.IsValid)
            {
                int cusID = _context.Customer.Where(x => x.UserName == User.Identity.Name).Select(c => c.Id).FirstOrDefault();

                int apartID = _context.Apartment.Where(x => x.DisplayName == apartment.DisplayName).Select(a => a.ApartmentId).FirstOrDefault();
                              
                Reservation res = new Reservation();
                res.ApartmentID = apartID;
                res.CustomerID = cusID;
                res.sAvailability = apartment.sAvailability;
                res.eAvailability = apartment.eAvailability;
                
                _context.Reservation.Add(res);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(apartment);
        }
        public async Task<IActionResult> ReservationExist(DateTime sdate, DateTime edate, int apartId)
        {
                var reservations = _context.Reservation.Where(x => ((sdate >= x.sAvailability && edate <= x.eAvailability) ||
                                                                   (sdate <= x.sAvailability && edate <= x.eAvailability) ||
                                                                   (sdate >= x.sAvailability && edate >= x.eAvailability) ||
                                                                   (sdate <= x.sAvailability && edate >= x.eAvailability)) && 
                                                                   (edate >= x.sAvailability && sdate <= x.eAvailability) && apartId == x.ApartmentID);         
            return Json(await reservations.ToListAsync());
        }

        public async Task<IActionResult> DelCheck(int AID)
        {
            var checkApart = _context.Reservation.Where(x => x.ApartmentID == AID);
            return Json(await checkApart.ToListAsync());
        }

        public void MakeTweet(string tweet)
        {
            string twitterURL = "https://api.twitter.com/1.1/statuses/update.json";

            string oauth_consumer_key = "wWLNkMnel0lgNEpSPqj49chPK";
            string oauth_consumer_secret = "AZPIvsJQ9TtCG0dZiu76smRhGRouEXgWvePn1xTyVDM0uEjob4";
            string oauth_token = "743233316-63W8j0i3b8rFaToH6J5nn2anIBluVOf2vs6x0Fx5";
            string oauth_token_secret = "9uDmM0omhUYsXXuM7bUwFB21is9lIwS1rFvoltTnC6nOA";

            // set the oauth version and signature method
            string oauth_version = "1.0";
            string oauth_signature_method = "HMAC-SHA1";

            // create unique request details
            string oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            System.TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            string oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            // create oauth signature
            string baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" + "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&status={6}";

            string baseString = string.Format(
                baseFormat,
                oauth_consumer_key,
                oauth_nonce,
                oauth_signature_method,
                oauth_timestamp, oauth_token,
                oauth_version,
                Uri.EscapeDataString(tweet)
            );

            string oauth_signature = null;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(Uri.EscapeDataString(oauth_consumer_secret) + "&" + Uri.EscapeDataString(oauth_token_secret))))
            {
                oauth_signature = Convert.ToBase64String(hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes("POST&" + Uri.EscapeDataString(twitterURL) + "&" + Uri.EscapeDataString(baseString))));
            }

            // create the request header
            string authorizationFormat = "OAuth oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", " + "oauth_signature=\"{2}\", oauth_signature_method=\"{3}\", " + "oauth_timestamp=\"{4}\", oauth_token=\"{5}\", " + "oauth_version=\"{6}\"";

            string authorizationHeader = string.Format(
                authorizationFormat,
                Uri.EscapeDataString(oauth_consumer_key),
                Uri.EscapeDataString(oauth_nonce),
                Uri.EscapeDataString(oauth_signature),
                Uri.EscapeDataString(oauth_signature_method),
                Uri.EscapeDataString(oauth_timestamp),
                Uri.EscapeDataString(oauth_token),
                Uri.EscapeDataString(oauth_version)
            );

            HttpWebRequest objHttpWebRequest = (HttpWebRequest)WebRequest.Create(twitterURL);
            objHttpWebRequest.Headers.Add("Authorization", authorizationHeader);
            objHttpWebRequest.Method = "POST";
            objHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            using (Stream objStream = objHttpWebRequest.GetRequestStream())
            {
                byte[] content = ASCIIEncoding.ASCII.GetBytes("status=" + Uri.EscapeDataString(tweet));
                objStream.Write(content, 0, content.Length);
            }
            var responseResult = "";

            try
            {
                //success posting
                WebResponse objWebResponse = objHttpWebRequest.GetResponse();
                StreamReader objStreamReader = new StreamReader(objWebResponse.GetResponseStream());
                responseResult = objStreamReader.ReadToEnd().ToString();
            }
            catch (Exception ex)
            {
                responseResult = "Twitter Post Error: " + ex.Message.ToString() + ", authHeader: " + authorizationHeader;
            }
        }

        public IActionResult Stats()
        {
            return View();
        }

        public IActionResult ShowCountriesGraph(string id)
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
            TempData["title"] = id;

            return View("Graph");
        }

        public IActionResult ShowAvgPriceGraph(string id)
        {
   
            var apartments = from address in _context.ApartmentAddress.AsEnumerable()
                             group address by address.Country into g
                             join apartment in _context.Apartment on g.FirstOrDefault().ApartmentAddressId equals apartment.ApartmentId
                             select new { key = g.Key, avg = g.Average(x => x.Apartment.Price) };

            var stats = apartments.Select(g => new
            {
                key = g.key,
                value = g.avg
            });

            TempData["graphData"] = JsonSerializer.Serialize(stats.ToList());
            TempData["title"] = id;

            return View("Graph");
        }
    }
}