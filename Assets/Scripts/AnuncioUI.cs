using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnuncioUI : MonoBehaviour
{
    public static AnuncioUI Instancia { get; private set; }

    [Header("UI")]
    public Text textoAnuncio;

    [Header("Tiempos")]
    public float duracionVisible = 1.2f;
    public float duracionFadeOut = 0.6f;

    [Header("Sonido (opcional)")]
    public AudioClip sonidoContraataque;

    private AudioSource _audioSource;
    private Coroutine _coroutineActual;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;

        _audioSource = GetComponent<AudioSource>();
        if (textoAnuncio != null)
            textoAnuncio.canvasRenderer.SetAlpha(0f);
    }

    public static void Mostrar(string texto, Color color)
    {
        if (Instancia == null) return;
        Instancia.MostrarAnuncio(texto, color);
    }

    private void MostrarAnuncio(string texto, Color color)
    {
        if (_coroutineActual != null)
            StopCoroutine(_coroutineActual);

        _coroutineActual = StartCoroutine(AnimarAnuncio(texto, color));
    }

    private IEnumerator AnimarAnuncio(string texto, Color color)
    {
        if (textoAnuncio == null) yield break;

        textoAnuncio.text = texto;
        textoAnuncio.color = color;
        textoAnuncio.canvasRenderer.SetAlpha(1f);

        if (_audioSource != null && sonidoContraataque != null)
            _audioSource.PlayOneShot(sonidoContraataque);

        yield return new WaitForSeconds(duracionVisible);

        float elapsed = 0f;
        while (elapsed < duracionFadeOut)
        {
            float alpha = 1f - (elapsed / duracionFadeOut);
            textoAnuncio.canvasRenderer.SetAlpha(alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        textoAnuncio.canvasRenderer.SetAlpha(0f);
        _coroutineActual = null;
    }
}
