using AcademiaDoZe.Application.DTOs;
using Microsoft.Maui.Controls;
using System.Globalization;

namespace AcademiaDoZe.Presentation.AppMaui.Converters;

public sealed class FotoToImageSourceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ArquivoDto { Conteudo.Length: > 0 } foto)
        {
            var bytes = foto.Conteudo;
            return ImageSource.FromStream(() => new MemoryStream(bytes, writable: false));
        }
        return null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException("O conversor de foto é unidirecional.");
}
//Aluno: Túlio Thauã Dutra
