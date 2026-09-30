using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SalesPurchaseManagement.Models;
using SalesPurchaseManagement.Repositories;

namespace SalesPurchaseManagement.Services;

public class MemberService
{
    private readonly MemberRepository _repository;

    public MemberService() => _repository = new MemberRepository();
    public MemberService(MemberRepository repository) => _repository = repository;

    public async Task<List<Member>> GetAllAsync() => await _repository.GetAllAsync();

    public async Task<int> CreateAsync(Member member)
    {
        if (string.IsNullOrWhiteSpace(member.MemberName))
            throw new ArgumentException("Member Name is required.");

        if (member.Discount < 0 || member.Discount > 100)
            throw new ArgumentException("Discount must be between 0% and 100%.");

        return await _repository.InsertAsync(member);
    }

    public async Task UpdateAsync(Member member)
    {
        if (member.MemberID <= 0)
            throw new ArgumentException("Valid Member ID required for update.");

        if (string.IsNullOrWhiteSpace(member.MemberName))
            throw new ArgumentException("Member Name is required.");

        if (member.Discount < 0 || member.Discount > 100)
            throw new ArgumentException("Discount must be between 0% and 100%.");

        await _repository.UpdateAsync(member);
    }

    public async Task DeleteAsync(int memberId)
    {
        try
        {
            await _repository.DeleteAsync(memberId);
        }
        catch (OracleException oex) when (oex.Number == 2292)
        {
            throw new InvalidOperationException("Cannot delete this membership tier because customers are currently assigned to it.", oex);
        }
    }
}
