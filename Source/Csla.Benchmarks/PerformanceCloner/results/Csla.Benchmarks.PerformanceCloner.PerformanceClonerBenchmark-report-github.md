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
| Method                | Job                  | Runtime              | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1      | Gen2      | Allocated | Alloc Ratio |
|---------------------- |--------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|-----------:|----------:|----------:|----------:|------------:|
| FetchAndSerialize     | .NET 10.0            | .NET 10.0            | 357.6 ms |  7.33 ms | 21.25 ms |  1.00 |    0.08 | 18000.0000 | 9000.0000 | 1000.0000 | 116.48 MB |        1.00 |
| FetchAndCloneInternal | .NET 10.0            | .NET 10.0            | 204.5 ms |  4.57 ms | 13.20 ms |  0.57 |    0.05 | 10000.0000 | 4000.0000 |         - |  67.29 MB |        0.58 |
|                       |                      |                      |          |          |          |       |         |            |           |           |           |             |
| FetchAndSerialize     | .NET 8.0             | .NET 8.0             | 393.9 ms |  7.32 ms | 13.93 ms |  1.00 |    0.05 | 20000.0000 | 9000.0000 | 1000.0000 | 130.34 MB |        1.00 |
| FetchAndCloneInternal | .NET 8.0             | .NET 8.0             | 243.0 ms |  4.85 ms | 13.37 ms |  0.62 |    0.04 | 13000.0000 | 5000.0000 |         - |  81.16 MB |        0.62 |
|                       |                      |                      |          |          |          |       |         |            |           |           |           |             |
| FetchAndSerialize     | .NET 9.0             | .NET 9.0             | 385.5 ms |  7.69 ms | 13.47 ms |  1.00 |    0.05 | 20000.0000 | 9000.0000 | 1000.0000 |  129.8 MB |        1.00 |
| FetchAndCloneInternal | .NET 9.0             | .NET 9.0             | 238.1 ms |  6.44 ms | 18.79 ms |  0.62 |    0.05 | 13000.0000 | 6000.0000 |         - |  80.63 MB |        0.62 |
|                       |                      |                      |          |          |          |       |         |            |           |           |           |             |
| FetchAndSerialize     | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 522.2 ms | 10.23 ms | 24.91 ms |  1.00 |    0.07 | 20000.0000 | 8000.0000 | 1000.0000 | 126.85 MB |        1.00 |
| FetchAndCloneInternal | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 356.8 ms |  7.60 ms | 22.06 ms |  0.68 |    0.05 | 12000.0000 | 4000.0000 |         - |  77.56 MB |        0.61 |
|                       |                      |                      |          |          |          |       |         |            |           |           |           |             |
| FetchAndSerialize     | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 546.0 ms | 10.89 ms | 29.82 ms |  1.00 |    0.08 | 20000.0000 | 8000.0000 | 1000.0000 | 126.85 MB |        1.00 |
| FetchAndCloneInternal | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 377.3 ms | 10.65 ms | 30.90 ms |  0.69 |    0.07 | 13000.0000 | 5000.0000 |  500.0000 |  77.67 MB |        0.61 |
|                       |                      |                      |          |          |          |       |         |            |           |           |           |             |
| FetchAndSerialize     | .NET Framework 4.8   | .NET Framework 4.8   | 516.1 ms | 15.92 ms | 46.17 ms |  1.01 |    0.12 | 20000.0000 | 8000.0000 | 1000.0000 | 126.85 MB |        1.00 |
| FetchAndCloneInternal | .NET Framework 4.8   | .NET Framework 4.8   | 363.1 ms |  7.91 ms | 22.32 ms |  0.71 |    0.07 | 13000.0000 | 5000.0000 |  500.0000 |  77.67 MB |        0.61 |
