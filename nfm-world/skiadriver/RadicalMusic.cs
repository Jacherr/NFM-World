using ManagedBass;

namespace NFMWorld.SkiaDriver;

using System.IO.Compression;
using NFMWorld.DriverInterface;
using File = Util.File;

internal class RadicalMusic : IRadicalMusic
{
    private bool _readable;
    private readonly int _music;

    public RadicalMusic(File file)
    {
#if USE_BASS
        using var fileStream = System.IO.File.OpenRead(file.Path);
        using var zipStream = new ZipArchive(fileStream, ZipArchiveMode.Read);
        using var resultStream = new MemoryStream();

        zipStream.Entries.First().Open().CopyTo(resultStream);
        var arr = resultStream.ToArray();
        if ((_music = Bass.MusicLoad(arr, 0, arr.Length, BassFlags.Loop)) == 0)
        {
            // it ain't playable
            throw new Exception(SoundClip.GetBassError(Bass.LastError));
        }
        _readable = true;
        //SetVolume(GameSparker.Volume);
        SetVolume(1.0f);
#endif
    }

    public RadicalMusic()
    {
        // empty
    }

    public void SetPaused(bool p0)
    {
#if USE_BASS
        if (!_readable) return;
        if (p0) Bass.ChannelPause(_music);
        else Bass.ChannelPlay(_music);
#endif
    }

    public void Unload()
    {
#if USE_BASS
        if (!_readable) return;
        Bass.MusicFree(_music);
        _readable = false;
#endif
    }

    public void Play()
    {
#if USE_BASS
        if (!_readable) return;
        Bass.ChannelPlay(_music);
#endif
    }

    public void SetVolume(float vol)
    {
#if USE_BASS
        if (!_readable) return;
        Bass.ChannelSetAttribute(_music, ChannelAttribute.Volume, vol);
#endif
    }
}