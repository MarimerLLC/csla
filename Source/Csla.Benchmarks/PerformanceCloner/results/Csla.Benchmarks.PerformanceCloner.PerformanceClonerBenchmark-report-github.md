```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.6466/22H2/2022Update) (Hyper-V)
Intel Core i7-10750H CPU 2.60GHz (Max: 2.59GHz), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host]               : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3
  .NET 10.0            : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3
  .NET 8.0             : .NET 8.0.27 (8.0.27, 8.0.2726.22922), X64 RyuJIT x86-64-v3
  .NET 9.0             : .NET 9.0.16 (9.0.16, 9.0.1626.22923), X64 RyuJIT x86-64-v3
  .NET Framework 4.6.2 : .NET Framework 4.8.1 (4.8.9310.0), X64 RyuJIT VectorSize=256
  .NET Framework 4.7.2 : .NET Framework 4.8.1 (4.8.9310.0), X64 RyuJIT VectorSize=256
  .NET Framework 4.8   : .NET Framework 4.8.1 (4.8.9310.0), X64 RyuJIT VectorSize=256


```
| Method                | Job                  | Runtime              | Mean     | Error    | StdDev   | Median   | Ratio | RatioSD | Gen0       | Gen1      | Gen2      | Allocated | Alloc Ratio |
|---------------------- |--------------------- |--------------------- |---------:|---------:|---------:|---------:|------:|--------:|-----------:|----------:|----------:|----------:|------------:|
| FetchAndSerialize     | .NET 10.0            | .NET 10.0            | 175.3 ms |  9.68 ms | 28.23 ms | 173.7 ms |  1.03 |    0.24 |  1000.0000 |         - |         - | 116.46 MB |        1.00 |
| FetchAndCloneInternal | .NET 10.0            | .NET 10.0            | 120.5 ms |  2.39 ms |  5.94 ms | 119.4 ms |  0.71 |    0.12 |   500.0000 |         - |         - |  67.28 MB |        0.58 |
|                       |                      |                      |          |          |          |          |       |         |            |           |           |           |             |
| FetchAndSerialize     | .NET 8.0             | .NET 8.0             | 223.5 ms | 12.77 ms | 37.67 ms | 225.2 ms |  1.03 |    0.25 |  2000.0000 | 1000.0000 |         - | 130.33 MB |        1.00 |
| FetchAndCloneInternal | .NET 8.0             | .NET 8.0             | 155.2 ms |  5.08 ms | 14.66 ms | 154.5 ms |  0.72 |    0.14 |   500.0000 |         - |         - |  81.15 MB |        0.62 |
|                       |                      |                      |          |          |          |          |       |         |            |           |           |           |             |
| FetchAndSerialize     | .NET 9.0             | .NET 9.0             | 214.3 ms | 13.04 ms | 38.46 ms | 214.8 ms |  1.04 |    0.28 |  2000.0000 | 1000.0000 |         - | 129.79 MB |        1.00 |
| FetchAndCloneInternal | .NET 9.0             | .NET 9.0             | 145.0 ms |  5.20 ms | 15.16 ms | 143.4 ms |  0.70 |    0.16 |   500.0000 |         - |         - |  80.61 MB |        0.62 |
|                       |                      |                      |          |          |          |          |       |         |            |           |           |           |             |
| FetchAndSerialize     | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 556.5 ms | 11.02 ms | 25.53 ms | 554.4 ms |  1.00 |    0.06 | 20000.0000 | 8000.0000 | 1000.0000 | 126.85 MB |        1.00 |
| FetchAndCloneInternal | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 360.0 ms |  8.81 ms | 25.27 ms | 358.5 ms |  0.65 |    0.05 | 12000.0000 | 4000.0000 |         - |  77.56 MB |        0.61 |
|                       |                      |                      |          |          |          |          |       |         |            |           |           |           |             |
| FetchAndSerialize     | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 563.1 ms | 11.17 ms | 18.67 ms | 562.1 ms |  1.00 |    0.05 | 20000.0000 | 8000.0000 | 1000.0000 | 126.85 MB |        1.00 |
| FetchAndCloneInternal | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 353.6 ms |  7.00 ms | 16.76 ms | 351.2 ms |  0.63 |    0.04 | 12000.0000 | 4000.0000 |         - |  77.56 MB |        0.61 |
|                       |                      |                      |          |          |          |          |       |         |            |           |           |           |             |
| FetchAndSerialize     | .NET Framework 4.8   | .NET Framework 4.8   | 579.6 ms | 17.85 ms | 51.79 ms | 562.9 ms |  1.01 |    0.12 | 20000.0000 | 8000.0000 | 1000.0000 | 126.85 MB |        1.00 |
| FetchAndCloneInternal | .NET Framework 4.8   | .NET Framework 4.8   | 340.3 ms |  6.65 ms |  8.88 ms | 340.6 ms |  0.59 |    0.05 | 12000.0000 | 4000.0000 |         - |  77.56 MB |        0.61 |
