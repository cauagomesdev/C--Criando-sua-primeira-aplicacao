using Adopet.Data;
using Adopet.Models;

namespace Adopet.Repositories;

public class TutorRepository
{
    private readonly AdopetContext _dbContext;

    public TutorRepository(AdopetContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Tutor? GetById(long id)
    {
        return _dbContext.Tutores.FirstOrDefault(t => t.Id == id);
    }

    public IEnumerable<Tutor> GetAll()
    {
        return _dbContext.Tutores.ToList();
    }

    public void Add(Tutor tutor)
    {
        _dbContext.Tutores.Add(tutor);
        _dbContext.SaveChanges();
    }
}
