```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4652/24H2/2024Update/HudsonValley)
AMD Ryzen 7 5800X 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.304
  [Host]               : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  .NET 8.0             : .NET 8.0.19 (8.0.1925.36514), X64 RyuJIT AVX2
  .NET 9.0             : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  .NET Framework 4.6.2 : .NET Framework 4.8.1 (4.8.9310.0), X64 RyuJIT VectorSize=256
  .NET Framework 4.7.2 : .NET Framework 4.8.1 (4.8.9310.0), X64 RyuJIT VectorSize=256
  .NET Framework 4.8   : .NET Framework 4.8.1 (4.8.9310.0), X64 RyuJIT VectorSize=256


```
| Method | Job                  | Runtime              | NumberOfItems | Mean            | Error         | StdDev        | Ratio | RatioSD |
|------- |--------------------- |--------------------- |-------------- |----------------:|--------------:|--------------:|------:|--------:|
| **Create** | **.NET 8.0**             | **.NET 8.0**             | **1**             |        **30.42 μs** |      **0.364 μs** |      **0.340 μs** |  **1.31** |    **0.02** |
| Create | .NET 9.0             | .NET 9.0             | 1             |        23.20 μs |      0.209 μs |      0.232 μs |  1.00 |    0.01 |
| Create | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 1             |        70.60 μs |      0.264 μs |      0.234 μs |  3.04 |    0.03 |
| Create | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 1             |        69.25 μs |      0.393 μs |      0.348 μs |  2.98 |    0.03 |
| Create | .NET Framework 4.8   | .NET Framework 4.8   | 1             |        70.08 μs |      0.502 μs |      0.470 μs |  3.02 |    0.04 |
|        |                      |                      |               |                 |               |               |       |         |
| **Create** | **.NET 8.0**             | **.NET 8.0**             | **10**            |        **66.34 μs** |      **0.747 μs** |      **0.831 μs** |  **1.17** |    **0.03** |
| Create | .NET 9.0             | .NET 9.0             | 10            |        56.75 μs |      0.906 μs |      1.300 μs |  1.00 |    0.03 |
| Create | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 10            |       164.73 μs |      0.560 μs |      0.524 μs |  2.90 |    0.06 |
| Create | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 10            |       165.49 μs |      1.062 μs |      0.993 μs |  2.92 |    0.06 |
| Create | .NET Framework 4.8   | .NET Framework 4.8   | 10            |       163.71 μs |      0.607 μs |      0.568 μs |  2.89 |    0.06 |
|        |                      |                      |               |                 |               |               |       |         |
| **Create** | **.NET 8.0**             | **.NET 8.0**             | **100**           |       **418.63 μs** |      **3.784 μs** |      **3.160 μs** |  **1.10** |    **0.03** |
| Create | .NET 9.0             | .NET 9.0             | 100           |       380.70 μs |      7.535 μs |     11.507 μs |  1.00 |    0.04 |
| Create | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 100           |     1,104.12 μs |     21.860 μs |     21.469 μs |  2.90 |    0.10 |
| Create | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 100           |     1,067.95 μs |      3.589 μs |      3.182 μs |  2.81 |    0.08 |
| Create | .NET Framework 4.8   | .NET Framework 4.8   | 100           |     1,065.52 μs |      5.328 μs |      4.984 μs |  2.80 |    0.08 |
|        |                      |                      |               |                 |               |               |       |         |
| **Create** | **.NET 8.0**             | **.NET 8.0**             | **1000**          |     **4,651.62 μs** |     **68.860 μs** |     **61.043 μs** |  **1.08** |    **0.02** |
| Create | .NET 9.0             | .NET 9.0             | 1000          |     4,311.18 μs |     32.509 μs |     30.409 μs |  1.00 |    0.01 |
| Create | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 1000          |    17,012.72 μs |    324.385 μs |    347.088 μs |  3.95 |    0.08 |
| Create | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 1000          |    16,732.66 μs |    330.092 μs |    366.896 μs |  3.88 |    0.09 |
| Create | .NET Framework 4.8   | .NET Framework 4.8   | 1000          |    16,337.16 μs |    132.756 μs |    110.857 μs |  3.79 |    0.04 |
|        |                      |                      |               |                 |               |               |       |         |
| **Create** | **.NET 8.0**             | **.NET 8.0**             | **10000**         |   **119,235.24 μs** |  **2,308.136 μs** |  **2,919.054 μs** |  **1.04** |    **0.04** |
| Create | .NET 9.0             | .NET 9.0             | 10000         |   114,738.22 μs |  2,241.116 μs |  3,141.732 μs |  1.00 |    0.04 |
| Create | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 10000         |   205,838.74 μs |  2,556.223 μs |  2,391.093 μs |  1.80 |    0.05 |
| Create | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 10000         |   205,994.46 μs |  2,688.964 μs |  2,515.259 μs |  1.80 |    0.05 |
| Create | .NET Framework 4.8   | .NET Framework 4.8   | 10000         |   203,591.04 μs |  1,383.315 μs |  1,226.273 μs |  1.78 |    0.05 |
|        |                      |                      |               |                 |               |               |       |         |
| **Create** | **.NET 8.0**             | **.NET 8.0**             | **100000**        | **1,134,581.03 μs** |  **8,213.392 μs** |  **7,280.958 μs** |  **0.99** |    **0.01** |
| Create | .NET 9.0             | .NET 9.0             | 100000        | 1,142,227.88 μs |  8,870.622 μs |  7,407.371 μs |  1.00 |    0.01 |
| Create | .NET Framework 4.6.2 | .NET Framework 4.6.2 | 100000        | 1,919,307.17 μs | 15,593.454 μs | 14,586.126 μs |  1.68 |    0.02 |
| Create | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 100000        | 1,937,460.75 μs | 33,454.070 μs | 29,656.162 μs |  1.70 |    0.03 |
| Create | .NET Framework 4.8   | .NET Framework 4.8   | 100000        | 2,028,175.96 μs | 39,296.108 μs | 32,814.030 μs |  1.78 |    0.03 |
