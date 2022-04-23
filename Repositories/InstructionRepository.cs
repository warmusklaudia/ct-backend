namespace Caketime.Repositories;

public interface IInstructionRepository
{
    Task<Instruction> AddInstruction(Instruction newInstruction);
    Task<List<Instruction>> AddInstructions(List<Instruction> newInstructions);
    Task<List<Instruction>> GetInstructions();
}

public class InstructionRepository : IInstructionRepository
{
    private readonly IMongoContext _context;

    public InstructionRepository(IMongoContext context)
    {
        _context = context;
    }

    public async Task<List<Instruction>> GetInstructions() => await _context.InstructionCollection.Find(_ => true).ToListAsync();
    public async Task<List<Instruction>> AddInstructions(List<Instruction> newInstructions)
    {
        await _context.InstructionCollection.InsertManyAsync(newInstructions);
        return newInstructions;
    }

    public async Task<Instruction> AddInstruction(Instruction newInstruction)
    {
        await _context.InstructionCollection.InsertOneAsync(newInstruction);
        return newInstruction;
    }
}