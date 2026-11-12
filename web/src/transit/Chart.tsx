import { useLayoutEffect, useMemo, useRef, useState, type PointerEvent } from 'react'

export interface ChartSeries {
  label: string
  x: number[]
  y: number[]
  color: string
  /** Dots for measurements, a line for a curve such as a periodogram. */
  kind: 'dots' | 'line'
}

/** A vertical reference line, e.g. a candidate period or a predicted transit. */
export interface ChartMarker {
  x: number
  color: string
  label?: string
  dashed?: boolean
  /** Draw a short tick at the bottom instead of a full-height line (for many markers). */
  tick?: boolean
}

interface ChartProps {
  ariaLabel: string
  series: ChartSeries[]
  markers?: ChartMarker[]
  xLabel: string
  yLabel: string
  xLog?: boolean
  xDomain?: [number, number]
  height?: number
  formatX: (value: number) => string
  formatY: (value: number) => string
}

const MARGIN = { top: 18, right: 14, bottom: 40, left: 64 }

/**
 * Small SVG chart for dense scientific series: one x axis, one y axis, recessive grid,
 * a crosshair with a tooltip on hover or touch. The y range ignores the extreme 0.5% of
 * points at each end so a few outliers do not flatten the transits.
 */
export function Chart({ ariaLabel, series, markers = [], xLabel, yLabel, xLog = false, xDomain, height = 240, formatX, formatY }: ChartProps) {
  const container = useRef<HTMLDivElement>(null)
  // 0 until measured, so an unmeasured chart never widens its container.
  const [width, setWidth] = useState(0)
  const [hover, setHover] = useState<{ series: number; index: number } | null>(null)

  useLayoutEffect(() => {
    const element = container.current
    if (!element) return
    // Measure now (before paint), then follow resizes.
    setWidth(Math.max(240, element.clientWidth))
    const observer = new ResizeObserver(([entry]) => setWidth(Math.max(240, entry.contentRect.width)))
    observer.observe(element)
    return () => observer.disconnect()
  }, [])

  const plotWidth = width - MARGIN.left - MARGIN.right
  const plotHeight = height - MARGIN.top - MARGIN.bottom

  const scales = useMemo(() => {
    const xs = series.flatMap((s) => s.x).filter((v) => !xLog || v > 0)
    const [x0, x1] = xDomain ?? [Math.min(...xs), Math.max(...xs)]
    const [y0, y1] = robustRange(series.flatMap((s) => s.y))

    const toX = xLog
      ? (v: number) => ((Math.log10(v) - Math.log10(x0)) / (Math.log10(x1) - Math.log10(x0))) * plotWidth
      : (v: number) => ((v - x0) / (x1 - x0 || 1)) * plotWidth
    const toY = (v: number) => plotHeight - ((v - y0) / (y1 - y0 || 1)) * plotHeight

    return {
      toX,
      toY,
      xTicks: xLog ? logTicks(x0, x1) : niceTicks(x0, x1, Math.max(3, Math.round(plotWidth / 90))),
      yTicks: niceTicks(y0, y1, Math.max(3, Math.round(plotHeight / 50))),
      domain: [x0, x1] as const,
    }
  }, [series, xLog, xDomain, plotWidth, plotHeight])

  const paths = useMemo(
    () =>
      series.map((s) => {
        let d = ''
        for (let i = 0; i < s.x.length; i++) {
          if (xLog && s.x[i] <= 0) continue
          const px = scales.toX(s.x[i]).toFixed(1)
          const py = scales.toY(s.y[i]).toFixed(1)
          // A zero-length segment with round caps draws a dot; one path keeps thousands of points cheap.
          d += s.kind === 'dots' ? `M${px} ${py}h0` : `${d ? 'L' : 'M'}${px} ${py}`
        }
        return d
      }),
    [series, scales, xLog],
  )

  const onPointerMove = (event: PointerEvent<SVGRectElement>) => {
    const rect = event.currentTarget.getBoundingClientRect()
    const px = event.clientX - rect.left
    let best: { series: number; index: number; distance: number } | null = null
    series.forEach((s, si) => {
      const index = nearestIndex(s.x, (x) => scales.toX(x), px)
      if (index < 0) return
      const distance = Math.abs(scales.toX(s.x[index]) - px)
      if (!best || distance < best.distance) best = { series: si, index, distance }
    })
    setHover(best)
  }

  const hovered = hover && series[hover.series]
  const hx = hovered ? scales.toX(hovered.x[hover!.index]) : 0
  const hy = hovered ? scales.toY(hovered.y[hover!.index]) : 0
  const clipId = useMemo(() => `clip-${Math.random().toString(36).slice(2)}`, [])

  return (
    <div className="chart" ref={container} style={{ height }}>
      {width > 0 && (
      <svg width={width} height={height} role="img" aria-label={ariaLabel}>
        <defs>
          <clipPath id={clipId}>
            <rect width={plotWidth} height={plotHeight} />
          </clipPath>
        </defs>
        <g transform={`translate(${MARGIN.left},${MARGIN.top})`}>
          {scales.yTicks.map((v) => (
            <g key={`y${v}`} transform={`translate(0,${scales.toY(v)})`}>
              <line className="chart__grid" x2={plotWidth} />
              <text className="chart__tick" x={-8} dy="0.32em" textAnchor="end">{formatY(v)}</text>
            </g>
          ))}
          {scales.xTicks.map((v) => (
            <text key={`x${v}`} className="chart__tick" x={scales.toX(v)} y={plotHeight + 18} textAnchor="middle">
              {formatX(v)}
            </text>
          ))}
          <line className="chart__axis" y1={plotHeight} x2={plotWidth} y2={plotHeight} />

          <g clipPath={`url(#${clipId})`}>
            {markers.map((m, i) => (
              <line key={`m${i}`} x1={scales.toX(m.x)} x2={scales.toX(m.x)} y1={m.tick ? plotHeight - 10 : 0} y2={plotHeight}
                stroke={m.color} strokeWidth={1.5} strokeDasharray={m.dashed ? '4 4' : undefined} opacity={0.9} />
            ))}
            {paths.map((d, i) => (
              <path key={i} d={d} fill="none" stroke={series[i].color}
                strokeWidth={series[i].kind === 'dots' ? 2.5 : 2} strokeLinecap="round" strokeLinejoin="round" />
            ))}
          </g>
          {markers.filter((m) => m.label).map((m, i) => (
            <text key={`l${i}`} className="chart__marker-label" x={scales.toX(m.x) + 4} y={-6}>{m.label}</text>
          ))}

          {hovered && (
            <g pointerEvents="none">
              <line className="chart__crosshair" x1={hx} x2={hx} y2={plotHeight} />
              <circle cx={hx} cy={hy} r={4.5} fill={hovered.color} stroke="var(--bg)" strokeWidth={2} />
            </g>
          )}
          <rect width={plotWidth} height={plotHeight} fill="transparent"
            onPointerMove={onPointerMove} onPointerDown={onPointerMove} onPointerLeave={() => setHover(null)} />

          <text className="chart__label" x={plotWidth / 2} y={plotHeight + 34} textAnchor="middle">{xLabel}</text>
          <text className="chart__label" transform={`translate(${-MARGIN.left + 12},${plotHeight / 2}) rotate(-90)`} textAnchor="middle">{yLabel}</text>
        </g>
      </svg>
      )}

      {hovered && (
        <div className="chart__tooltip"
          style={{ left: Math.min(MARGIN.left + hx + 12, width - 170), top: Math.max(0, MARGIN.top + hy - 48) }}>
          {series.length > 1 && <div className="chart__tooltip-title">{hovered.label}</div>}
          <div>{xLabel}: <b>{formatX(hovered.x[hover!.index])}</b></div>
          <div>{yLabel}: <b>{formatY(hovered.y[hover!.index])}</b></div>
        </div>
      )}
    </div>
  )
}

/** Index of the point whose screen x is closest to px; assumes x is sorted ascending. */
function nearestIndex(x: number[], toScreen: (v: number) => number, px: number): number {
  if (x.length === 0) return -1
  let lo = 0
  let hi = x.length - 1
  while (hi - lo > 1) {
    const mid = (lo + hi) >> 1
    if (toScreen(x[mid]) < px) lo = mid
    else hi = mid
  }
  return Math.abs(toScreen(x[lo]) - px) <= Math.abs(toScreen(x[hi]) - px) ? lo : hi
}

function robustRange(values: number[]): [number, number] {
  const finite = values.filter(Number.isFinite).sort((a, b) => a - b)
  if (finite.length === 0) return [0, 1]
  const lo = finite[Math.floor(finite.length * 0.005)]
  const hi = finite[Math.ceil(finite.length * 0.995) - 1]
  const pad = (hi - lo || Math.abs(hi) || 1) * 0.08
  return [lo - pad, hi + pad]
}

function niceTicks(min: number, max: number, count: number): number[] {
  const span = max - min
  if (!(span > 0)) return [min]
  const raw = span / count
  const power = 10 ** Math.floor(Math.log10(raw))
  const step = [1, 2, 2.5, 5, 10].map((f) => f * power).find((s) => s >= raw) ?? raw
  const ticks: number[] = []
  for (let v = Math.ceil(min / step) * step; v <= max + step * 1e-9; v += step)
    ticks.push(Number(v.toPrecision(12)))
  return ticks
}

function logTicks(min: number, max: number): number[] {
  const ticks: number[] = []
  for (let p = Math.floor(Math.log10(min)); p <= Math.ceil(Math.log10(max)); p++)
    for (const f of [1, 2, 5]) {
      const v = f * 10 ** p
      if (v >= min && v <= max) ticks.push(Number(v.toPrecision(12)))
    }
  return ticks
}
