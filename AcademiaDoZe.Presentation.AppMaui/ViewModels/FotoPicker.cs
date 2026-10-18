using AcademiaDoZe.Application.DTOs;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public readonly record struct ResultadoFoto(bool Alterada, ArquivoDto? Foto);

public static class FotoPicker
{
    private const int TamanhoMaximoBytes = 8 * 1024 * 1024;

    public static async Task<ResultadoFoto> SelecionarAsync()
    {
        var opcao = await Shell.Current.DisplayActionSheetAsync(
            "Escolha a foto", "Cancelar", "Remover foto", "Galeria", "Câmera");

        if (opcao is null or "Cancelar")
            return new ResultadoFoto(false, null);
        if (opcao == "Remover foto")
            return new ResultadoFoto(true, null);

        FileResult? arquivo = null;
        if (opcao == "Galeria")
        {
            arquivo = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Selecione uma imagem",
                FileTypes = FilePickerFileType.Images
            });
        }
        else if (opcao == "Câmera")
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await Shell.Current.DisplayAlertAsync("Câmera indisponível", "Este dispositivo não oferece captura de fotos.", "OK");
                return new ResultadoFoto(false, null);
            }

            var permissao = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (permissao != PermissionStatus.Granted)
                permissao = await Permissions.RequestAsync<Permissions.Camera>();
            if (permissao != PermissionStatus.Granted)
            {
                await Shell.Current.DisplayAlertAsync("Permissão necessária", "Autorize o acesso à câmera nas configurações do dispositivo para tirar uma foto.", "OK");
                return new ResultadoFoto(false, null);
            }

            arquivo = await MediaPicker.Default.CapturePhotoAsync();
        }

        if (arquivo is null)
            return new ResultadoFoto(false, null);

        await using var origem = await arquivo.OpenReadAsync();
        using var destino = new MemoryStream();
        await origem.CopyToAsync(destino);
        if (destino.Length == 0)
            throw new InvalidDataException("A imagem selecionada está vazia.");
        if (destino.Length > TamanhoMaximoBytes)
            throw new InvalidDataException("A foto deve ter no máximo 8 MB. Escolha uma imagem menor.");

        return new ResultadoFoto(true, new ArquivoDto { Conteudo = destino.ToArray() });
    }
}
//Aluno: Túlio Thauã Dutra
