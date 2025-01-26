using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.ContractsContent;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

public class ContractContentHandler(AppDbContext context) : IContractContentHandler
{
    public async Task<Response<ContractContent?>> CreateAsync(CreateContractContentRequest request)
    {
        try
        {
            if (request.HttpRequest is null)
                return new Response<ContractContent?>(null, 400, "Requisição inválida");

            if (!request.HttpRequest.HasFormContentType)
                return new Response<ContractContent?>(null, 400,
                    "Conteúdo do tipo multipart/form-data esperado");

            var form = await request.HttpRequest.ReadFormAsync();
            var files = form.Files;

            if (files.Count == 0)
                return new Response<ContractContent?>(null, 400, "Nenhum arquivo encontrado");

            var contracts = new List<ContractContent>();

            foreach (var file in files)
            {
                var id = file.Headers["Id"].FirstOrDefault();
                var idPhoto = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{idPhoto}{extension}";
                var currentDirectory = Directory.GetCurrentDirectory();
                var fullFileName = Path.Combine(currentDirectory, "Sources", "Contracts", fileName);

                await using var stream = new FileStream(fullFileName, FileMode.Create);
                await file.CopyToAsync(stream);

                var contractContent = new ContractContent
                {
                    Id = idPhoto,
                    UserId = request.UserId,
                    Extension = extension,
                    Name = request.Name,
                    IsActive = true
                };
                contracts.Add(contractContent);
            }

            await context.ContractsContent.AddRangeAsync(contracts);
            await context.SaveChangesAsync();

            return new Response<ContractContent?>(null, 201);
        }
        catch
        {
            return new Response<ContractContent?>(null, 500, "Não foi possível salvar o modelo do contrato");
        }
    }

    public async Task<Response<ContractContent?>> UpdateAsync(UpdateContractContentRequest request)
    {
        try
        {
            var contractContent = await context
                .ContractsContent
                .FirstOrDefaultAsync(c =>
                    c.Id == request.Id
                    && c.UserId == request.UserId
                    && c.IsActive);

            if (contractContent is null)
                return new Response<ContractContent?>(null, 404, "Modelo do contrato não encontrado");

            if (request.HttpRequest is null)
                return new Response<ContractContent?>(null, 400, "Requisição inválida");

            if (!request.HttpRequest.HasFormContentType)
                return new Response<ContractContent?>(null, 400,
                    "Conteúdo do tipo multipart/form-data esperado");

            var form = await request.HttpRequest.ReadFormAsync();
            var files = form.Files;

            var contracts = new List<ContractContent>();

            foreach (var file in files)
            {
                var id = file.Headers["Id"].FirstOrDefault();
                var idPhoto = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
                var extension = Path.GetExtension(file.Name);
                var fileName = $"{idPhoto}{extension}";
                File.Delete(fileName);
                var currentDirectory = Directory.GetCurrentDirectory();
                var fullFileName = Path.Combine(currentDirectory, "Sources", "Contracts", fileName);

                await using var stream = new FileStream(fullFileName, FileMode.Create);
                await file.CopyToAsync(stream);

                var newContractContent = new ContractContent
                {
                    Id = idPhoto,
                    UserId = request.UserId,
                    Extension = extension,
                    Name = request.Name,
                    IsActive = true
                };
                contracts.Add(newContractContent);
            }

            context.ContractsContent.Remove(contractContent);
            await context.ContractsContent.AddRangeAsync(contracts);
            await context.SaveChangesAsync();

            return new Response<ContractContent?>(null, 204);
        }
        catch
        {
            return new Response<ContractContent?>(null, 500, "Não foi possível atualizar o modelo do contrato");
        }
    }

    public async Task<Response<ContractContent?>> DeleteAsync(DeleteContractContentRequest request)
    {
        try
        {
            var contractContent = await context
                .ContractsContent
                .FirstOrDefaultAsync(c =>
                    c.Id == request.Id
                    && c.UserId == request.UserId
                    && c.IsActive);

            if (contractContent is null)
                return new Response<ContractContent?>(null, 404, "Modelo do contrato não encontrado");

            contractContent.IsActive = false;
            context.ContractsContent.Update(contractContent);
            await context.SaveChangesAsync();

            return new Response<ContractContent?>(null, 204);
        }
        catch
        {
            return new Response<ContractContent?>(null, 500, "Não foi possível deletar o modelo do contrato");
        }
    }

    public async Task<Response<List<ContractContent>?>> GetAllByUserAsync(GetAllContractContentByUserRequest request)
    {
        try
        {
            var contracts = await context
                .ContractsContent
                .AsNoTracking()
                .Where(c => c.UserId == request.UserId && c.IsActive)
                .ToListAsync();

            return new Response<List<ContractContent>?>(contracts);
        }
        catch
        {
            return new Response<List<ContractContent>?>(null, 500, "Não foi possível buscar os modelos de contrato");
        }
    }
}