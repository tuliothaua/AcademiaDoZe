using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using LogradouroEntity = AcademiaDoZe.Domain.Entities.Logradouro;
using AlunoEntity = AcademiaDoZe.Domain.Entities.Aluno;
using ColaboradorEntity = AcademiaDoZe.Domain.Entities.Colaborador;

namespace AcademiaDoZe.Domain.Tests;

public static class DomainTestData
{
    public static Arquivo Foto()
    {
        return Arquivo.Criar(new byte[] { 1, 2, 3 }).Value!;
    }

    public static Logradouro Logradouro()
    {
        return LogradouroEntity.Criar(1, "12345678", "Brasil", "SP", "Sao Paulo", "Centro", "Rua A").Value!;
    }

    public static Aluno Aluno()
    {
        return AlunoEntity.Criar(
            2,
            "Aluno Teste",
            "12345678901",
            DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
            "11987654321",
            "aluno@academia.com",
            "senha123",
            Foto(),
            Logradouro(),
            "10",
            "").Value!;
    }

    public static Colaborador Colaborador()
    {
        return ColaboradorEntity.Criar(
            3,
            "Colaborador Teste",
            "12345678901",
            DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
            "11987654321",
            "colaborador@academia.com",
            "senha123",
            Foto(),
            Logradouro(),
            "20",
            "",
            DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            ColaboradorTipo.Atendente,
            ColaboradorVinculo.CLT).Value!;
    }
}