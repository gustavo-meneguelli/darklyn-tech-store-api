using System.Globalization;
using System.Text;
using Application.Common.Interfaces;

namespace Infrastructure.Services;

public class ProfanityFilterService : IProfanityFilterService
{
    private readonly HashSet<string> _profanitySet;

    public ProfanityFilterService()
    {
        // Lista base de termos ofensivos em PT-BR
        // Esta lista é conservadora para evitar falsos positivos excessivos, 
        // mas abrange os termos mais comuns de alta gravidade.
        _profanitySet = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Palavrões Comuns
            "merda", "bosta", "pqp", "caralho", "porra", "puta", "puto", 
            "foda", "fodase", "fuder", "foder", "buceta", "caguei",
            "cu", "arrombado", "arrombada", "babaca", "cacete",
            "vai toma no seu cu", "vai toma no cu",
            
            // Ofensas / Slurs (Termos de ódio)
            "viado", "bicha", "sapatão", "traveco", "macaco", "preto", 
            "negro", "aleijado", "retardado", "idiota", "imbecil",
            "vagabundo", "vagabunda", "piranha", "kadel", "cadela",
            "nazista", "hitler", "estupro", "pedofilo", "pedofilia",
            
            // Variações comuns (Leetspeak simples)
            "fdp", "f.d.p", "vsf", "vtnc", "p.q.p"
        };
    }

    public bool ContainsProfanity(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var normalizedText = RemoveDiacritics(text).ToLower();
        var words = normalizedText.Split(new[] { ' ', '.', ',', '!', '?', ';', ':', '-', '_', '"', '\'' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words)
        {
            if (_profanitySet.Contains(word))
            {
                return true;
            }
        }

        // Verificação de frases específicas ou padrões compostos podem ser adicionados aqui
        return false;
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
