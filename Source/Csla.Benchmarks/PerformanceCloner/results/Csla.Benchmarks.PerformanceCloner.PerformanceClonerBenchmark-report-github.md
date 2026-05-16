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
| FetchAndSerialize     | .NET 10.0            | .NET 10.0            | 159.9 ms |  8.21 ms | 24.09 ms | 160.1 ms |  1.02 |    0.22 |          - |         - |         - | 116.46 MB |        1.00 |
| FetchAndCloneInternal | .NET 10.0            | .NET 10.0            | 111.2 ms |  2.22 ms |  5.93 ms | 109.9 ms |  0.71 |    0.12 |   500.0000 |         - |         - |  69.24 MB |        0.59 |
|                       |                      |                      |          |          |          |          |       |         |            |           |           |           |             |
| FetchAndSerialize     | .NET 8.0             | .NET 8.0             | 205.0 ms |  9.34 ms | 27.54 ms | 214.5 ms |  1.02 |    0.21 |  2000.0000 | 1000.0000 |         - | 130.33 MB |        1.00 |
| FetchAndCloneInternal | .NET 8.0             | .NET 8.0             | 158.7 ms |  5.37 ms | 15.57 ms | 159.5 ms |  0.79 |    0.14 |   500.0000 |         - |         - |  83.11 MB |        0.64 |
|                       |                      |                      |          |          |          |          |       |         |            |           |           |           |             |
| FetchAndSerialize     | .NET 9.0             | .NET 9.0             | 207.7 ms | 12.87 ms | 36.51 ms | 215.9 ms |  1.03 |    0.27 |  2000.0000 | 1000.0000 |         - | 129.79 MB |        1.00 |
| FetchAndCloneInternal | .NET 9.0             | .NET 9.0             | 135.9 ms |  5.46 ms | 16.10 ms | 135.2 ms |  0.68 |    0.16 |  1000.0000 |  500.0000 |         - |  82.57 MB |        0.64 |
|                       |                      |                      |          |          |          |          |       |         |            |           |           |           |             |
| FetchAndSerialize     | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 529.1 ms | 10.55 ms | 17.34 ms | 526.8 ms |  1.00 |    0.05 | 20000.0000 | 8000.0000 | 1000.0000 | 126.85 MB |        1.00 |
| FetchAndCloneInternal | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 356.0 ms |  8.72 ms | 25.03 ms | 352.2 ms |  0.67 |    0.05 | 13000.0000 | 5000.0000 | 1000.0000 |  79.72 MB |        0.63 |
|                       |                      |                      |          |          |          |          |       |         |            |           |           |           |             |
| FetchAndSerialize     | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 489.3 ms |  9.75 ms | 18.79 ms | 486.0 ms |  1.00 |    0.05 | 20000.0000 | 8000.0000 | 1000.0000 | 126.85 MB |        1.00 |
| FetchAndCloneInternal | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 337.0 ms |  6.72 ms |  8.97 ms | 337.5 ms |  0.69 |    0.03 | 13000.0000 | 5000.0000 | 1000.0000 |  79.72 MB |        0.63 |
|                       |                      |                      |          |          |          |          |       |         |            |           |           |           |             |
| FetchAndSerialize     | .NET Framework 4.8   | .NET Framework 4.8   | 482.8 ms |  9.47 ms | 13.59 ms | 481.0 ms |  1.00 |    0.04 | 20000.0000 | 8000.0000 | 1000.0000 | 126.85 MB |        1.00 |
| FetchAndCloneInternal | .NET Framework 4.8   | .NET Framework 4.8   | 334.8 ms |  6.66 ms |  6.23 ms | 336.6 ms |  0.69 |    0.02 | 13000.0000 | 5000.0000 | 1000.0000 |  79.65 MB |        0.63 |
