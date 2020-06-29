using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}",Name="GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if(celestialObject == null)
            {
                return NotFound();
            }

            var orbitalObjects = _context.CelestialObjects.Where(c => c.OrbitedObjectId == id).ToList();

            celestialObject.Satellites = orbitalObjects;

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Name == name);
            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            foreach (CelestialObject celestialObject in celestialObjects) 
            {
                var orbitalObjects = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
                celestialObject.Satellites = orbitalObjects;
            }
                        
            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects;

            foreach (CelestialObject celestialObject in celestialObjects)
            {
                var orbitalObjects = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
                celestialObject.Satellites = orbitalObjects;
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject) ;
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var objectToUpdate = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if(objectToUpdate == null)
                return NotFound();

            objectToUpdate.Name = celestialObject.Name;
            objectToUpdate.OrbitalPeriod = celestialObject.OrbitalPeriod;
            objectToUpdate.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(objectToUpdate);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var objectToUpdate = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (objectToUpdate == null)
                return NotFound();

            objectToUpdate.Name = name;
            _context.CelestialObjects.Update(objectToUpdate);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objectsToDelete = _context.CelestialObjects.Where(c => (c.Id == id || c.OrbitedObjectId == id));

            if (!objectsToDelete.Any())
                return NotFound();

            _context.CelestialObjects.RemoveRange(objectsToDelete);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
