using System;
using System.Collections.ObjectModel;
using Demo.RelativeControl.DataGrid.ViewModels;

namespace Demo.RelativeControl.ViewModels;


public class SampleData {
    public int Field1 { get; set; }
    public double Field2 { get; set; }
    public string Field3 { get; set; }
    public bool Field4 { get; set; }
    public DateTime Field5 { get; set; }
    public float Field6 { get; set; }
    public decimal Field7 { get; set; }
    public long Field8 { get; set; }
    public short Field9 { get; set; }
    public byte Field10 { get; set; }
    public uint Field11 { get; set; }
    public ulong Field12 { get; set; }
    public ushort Field13 { get; set; }
    public sbyte Field14 { get; set; }
    public char Field15 { get; set; }
    public int? Field16 { get; set; }
    public double? Field17 { get; set; }
    public bool? Field18 { get; set; }
    public DateTime? Field19 { get; set; }
    public float? Field20 { get; set; }
    public decimal? Field21 { get; set; }
    public long? Field22 { get; set; }
    public short? Field23 { get; set; }
    public byte? Field24 { get; set; }
    public uint? Field25 { get; set; }
    public ulong? Field26 { get; set; }
    public ushort? Field27 { get; set; }
    public sbyte? Field28 { get; set; }
    public char? Field29 { get; set; }
    public string? Field30 { get; set; }
    public int Field31 { get; set; }
    public double Field32 { get; set; }
    public string Field33 { get; set; }
    public bool Field34 { get; set; }
    public DateTime Field35 { get; set; }
    public float Field36 { get; set; }
    public decimal Field37 { get; set; }
    public long Field38 { get; set; }
    public short Field39 { get; set; }
    public byte Field40 { get; set; }
    public uint Field41 { get; set; }
    public ulong Field42 { get; set; }
    public ushort Field43 { get; set; }
    public sbyte Field44 { get; set; }
    public char Field45 { get; set; }
    public int? Field46 { get; set; }
    public double? Field47 { get; set; }
    public bool? Field48 { get; set; }
    public DateTime? Field49 { get; set; }
    public float? Field50 { get; set; }
    public decimal? Field51 { get; set; }
    public long? Field52 { get; set; }
    public short? Field53 { get; set; }
    public byte? Field54 { get; set; }
    public uint? Field55 { get; set; }
    public ulong? Field56 { get; set; }
    public ushort? Field57 { get; set; }
    public sbyte? Field58 { get; set; }
    public char? Field59 { get; set; }
    public string? Field60 { get; set; }
    public int Field61 { get; set; }
    public double Field62 { get; set; }
    public string Field63 { get; set; }
    public bool Field64 { get; set; }
    public DateTime Field65 { get; set; }
    public float Field66 { get; set; }
    public decimal Field67 { get; set; }
    public long Field68 { get; set; }
    public short Field69 { get; set; }
    public byte Field70 { get; set; }
    public uint Field71 { get; set; }
    public ulong Field72 { get; set; }
    public ushort Field73 { get; set; }
    public sbyte Field74 { get; set; }
    public char Field75 { get; set; }
    public int? Field76 { get; set; }
    public double? Field77 { get; set; }
    public bool? Field78 { get; set; }
    public DateTime? Field79 { get; set; }
    public float? Field80 { get; set; }
    public decimal? Field81 { get; set; }
    public long? Field82 { get; set; }
    public short? Field83 { get; set; }
    public byte? Field84 { get; set; }
    public uint? Field85 { get; set; }
    public ulong? Field86 { get; set; }
    public ushort? Field87 { get; set; }
    public sbyte? Field88 { get; set; }
    public char? Field89 { get; set; }
    public string? Field90 { get; set; }
    public int Field91 { get; set; }
    public double Field92 { get; set; }
    public string Field93 { get; set; }
    public bool Field94 { get; set; }
    public DateTime Field95 { get; set; }
    public float Field96 { get; set; }
    public decimal Field97 { get; set; }
    public long Field98 { get; set; }
    public short Field99 { get; set; }
    public byte Field100 { get; set; }
}


public class RelativeDataGridViewModel : ViewModelBase {
    public static SampleData[] Source { get; }
    
    static RelativeDataGridViewModel() {
        Source = new SampleData[400];
        for (int i = 0; i < 400; i++) {
            Source[i] = new SampleData {
                Field1 = i,
                Field2 = i * 0.1,
                Field3 = $"str_{i}",
                Field4 = i % 2 == 0,
                Field5 = DateTime.Now.AddDays(i),
                Field6 = (float)(i * 0.2),
                Field7 = (decimal)(i * 0.3),
                Field8 = i * 1000L,
                Field9 = (short)(i % 32768),
                Field10 = (byte)(i % 256),
                Field11 = (uint)i,
                Field12 = (ulong)i,
                Field13 = (ushort)(i % 65536),
                Field14 = (sbyte)(i % 128),
                Field15 = (char)('A' + (i % 26)),
                Field16 = i % 3 == 0 ? null : i,
                Field17 = i % 4 == 0 ? null : i * 0.1,
                Field18 = i % 5 == 0 ? null : i % 2 == 0,
                Field19 = i % 6 == 0 ? null : DateTime.Now.AddDays(i),
                Field20 = i % 7 == 0 ? null : (float)(i * 0.2),
                Field21 = i % 8 == 0 ? null : (decimal)(i * 0.3),
                Field22 = i % 9 == 0 ? null : i * 1000L,
                Field23 = i % 10 == 0 ? null : (short)(i % 32768),
                Field24 = i % 11 == 0 ? null : (byte)(i % 256),
                Field25 = i % 12 == 0 ? null : (uint)i,
                Field26 = i % 13 == 0 ? null : (ulong)i,
                Field27 = i % 14 == 0 ? null : (ushort)(i % 65536),
                Field28 = i % 15 == 0 ? null : (sbyte)(i % 128),
                Field29 = i % 16 == 0 ? null : (char)('A' + (i % 26)),
                Field30 = i % 17 == 0 ? null : $"str_{i}",
                Field31 = i + 1,
                Field32 = (i + 1) * 0.1,
                Field33 = $"str_{i + 1}",
                Field34 = (i + 1) % 2 == 0,
                Field35 = DateTime.Now.AddDays(i + 1),
                Field36 = (float)((i + 1) * 0.2),
                Field37 = (decimal)((i + 1) * 0.3),
                Field38 = (i + 1) * 1000L,
                Field39 = (short)((i + 1) % 32768),
                Field40 = (byte)((i + 1) % 256),
                Field41 = (uint)(i + 1),
                Field42 = (ulong)(i + 1),
                Field43 = (ushort)((i + 1) % 65536),
                Field44 = (sbyte)((i + 1) % 128),
                Field45 = (char)('A' + ((i + 1) % 26)),
                Field46 = (i + 1) % 3 == 0 ? null : i + 1,
                Field47 = (i + 1) % 4 == 0 ? null : (i + 1) * 0.1,
                Field48 = (i + 1) % 5 == 0 ? null : (i + 1) % 2 == 0,
                Field49 = (i + 1) % 6 == 0 ? null : DateTime.Now.AddDays(i + 1),
                Field50 = (i + 1) % 7 == 0 ? null : (float)((i + 1) * 0.2),
                Field51 = (i + 1) % 8 == 0 ? null : (decimal)((i + 1) * 0.3),
                Field52 = (i + 1) % 9 == 0 ? null : (i + 1) * 1000L,
                Field53 = (i + 1) % 10 == 0 ? null : (short)((i + 1) % 32768),
                Field54 = (i + 1) % 11 == 0 ? null : (byte)((i + 1) % 256),
                Field55 = (i + 1) % 12 == 0 ? null : (uint)(i + 1),
                Field56 = (i + 1) % 13 == 0 ? null : (ulong)(i + 1),
                Field57 = (i + 1) % 14 == 0 ? null : (ushort)((i + 1) % 65536),
                Field58 = (i + 1) % 15 == 0 ? null : (sbyte)((i + 1) % 128),
                Field59 = (i + 1) % 16 == 0 ? null : (char)('A' + ((i + 1) % 26)),
                Field60 = (i + 1) % 17 == 0 ? null : $"str_{i + 1}",
                Field61 = i + 2,
                Field62 = (i + 2) * 0.1,
                Field63 = $"str_{i + 2}",
                Field64 = (i + 2) % 2 == 0,
                Field65 = DateTime.Now.AddDays(i + 2),
                Field66 = (float)((i + 2) * 0.2),
                Field67 = (decimal)((i + 2) * 0.3),
                Field68 = (i + 2) * 1000L,
                Field69 = (short)((i + 2) % 32768),
                Field70 = (byte)((i + 2) % 256),
                Field71 = (uint)(i + 2),
                Field72 = (ulong)(i + 2),
                Field73 = (ushort)((i + 2) % 65536),
                Field74 = (sbyte)((i + 2) % 128),
                Field75 = (char)('A' + ((i + 2) % 26)),
                Field76 = (i + 2) % 3 == 0 ? null : i + 2,
                Field77 = (i + 2) % 4 == 0 ? null : (i + 2) * 0.1,
                Field78 = (i + 2) % 5 == 0 ? null : (i + 2) % 2 == 0,
                Field79 = (i + 2) % 6 == 0 ? null : DateTime.Now.AddDays(i + 2),
                Field80 = (i + 2) % 7 == 0 ? null : (float)((i + 2) * 0.2),
                Field81 = (i + 2) % 8 == 0 ? null : (decimal)((i + 2) * 0.3),
                Field82 = (i + 2) % 9 == 0 ? null : (i + 2) * 1000L,
                Field83 = (i + 2) % 10 == 0 ? null : (short)((i + 2) % 32768),
                Field84 = (i + 2) % 11 == 0 ? null : (byte)((i + 2) % 256),
                Field85 = (i + 2) % 12 == 0 ? null : (uint)(i + 2),
                Field86 = (i + 2) % 13 == 0 ? null : (ulong)(i + 2),
                Field87 = (i + 2) % 14 == 0 ? null : (ushort)((i + 2) % 65536),
                Field88 = (i + 2) % 15 == 0 ? null : (sbyte)((i + 2) % 128),
                Field89 = (i + 2) % 16 == 0 ? null : (char)('A' + ((i + 2) % 26)),
                Field90 = (i + 2) % 17 == 0 ? null : $"str_{i + 2}",
                Field91 = i + 3,
                Field92 = (i + 3) * 0.1,
                Field93 = $"str_{i + 3}",
                Field94 = (i + 3) % 2 == 0,
                Field95 = DateTime.Now.AddDays(i + 3),
                Field96 = (float)((i + 3) * 0.2),
                Field97 = (decimal)((i + 3) * 0.3),
                Field98 = (i + 3) * 1000L,
                Field99 = (short)((i + 3) % 32768),
                Field100 = (byte)((i + 3) % 256)
            };
        }
    }

}