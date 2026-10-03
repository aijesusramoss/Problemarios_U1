import 'dart:math' as math;

class ResultadoIntegral {
  final double valorAproximado;
  final int iteraciones;
  final int subintervalosFinales;
  final String codigoParada;

  ResultadoIntegral({
    required this.valorAproximado,
    required this.iteraciones,
    required this.subintervalosFinales,
    required this.codigoParada,
  });

  @override
  String toString() {
    return '''
================ RESULTADO ================
Aproximación de la integral: $valorAproximado
Iteraciones realizadas:    $iteraciones
Subintervalos finales (N): $subintervalosFinales
Código de parada:           $codigoParada
===========================================
''';
  }
}

/// Función a integrar: f(x) = x^3 * cos(x) + e^(x^2)
double funcionTarget(double x) {
  return math.pow(x, 3) * math.cos(x) + math.exp(math.pow(x, 2));
}

/// Implementación de la Regla de Simpson 1/3 para N subintervalos dada una función f
double simpson13(double Function(double) f, double a, double b, int n) {
  assert(n % 2 == 0, "El número de subintervalos N debe ser par.");

  double h = (b - a) / n;
  double sumaPares = 0.0;
  double sumaImpares = 0.0;

  for (int i = 1; i < n; i++) {
    double x = a + i * h;
    if (i % 2 == 0) {
      sumaPares += f(x);
    } else {
      sumaImpares += f(x);
    }
  }

  return (h / 3) * (f(a) + f(b) + 4 * sumaImpares + 2 * sumaPares);
}

ResultadoIntegral simpsonAdaptativo({
  required double Function(double) f,
  required double a,
  required double b,
  double toleracia = 1e-7,
  int maxN = 1048576,
}) {
  int n = 2;
  int iteraciones = 1;

  double iAnterior = simpson13(f, a, b, n);
  double iActual = iAnterior;

  String codigoParada = 'CONVERGENCIA';

  while (true) {
    if (n >= maxN) {
      codigoParada = 'LIMITE_ALCANZADO';
      break;
    }

    n *= 2;
    iteraciones++;

    iActual = simpson13(f, a, b, n);

    if ((iActual - iAnterior).abs() < toleracia) {
      codigoParada = 'CONVERGENCIA';
      break;
    }

    iAnterior = iActual;
  }

  return ResultadoIntegral(
    valorAproximado: iActual,
    iteraciones: iteraciones,
    subintervalosFinales: n,
    codigoParada: codigoParada,
  );
}

void main() {
  double a = 0.0;
  double b = 1.0;

  ResultadoIntegral resultado = simpsonAdaptativo(f: funcionTarget, a: a, b: b);

  print(resultado);
}
