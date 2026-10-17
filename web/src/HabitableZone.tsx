import React from 'react'

interface HabitableZoneProps {
  innerRadius: number // i AU
  outerRadius: number // i AU
  scaleDistanceFn: (au: number) => number
}

export const HabitableZone: React.FC<HabitableZoneProps> = ({
  innerRadius,
  outerRadius,
  scaleDistanceFn
}) => {
  const innerScaled = scaleDistanceFn(innerRadius)
  const outerScaled = scaleDistanceFn(outerRadius)

  return (
    <>
      {/* Inre sfär – röd, tunn, genomskinlig */}
      <mesh>
        <sphereGeometry args={[innerScaled, 64, 64]} />
        <meshBasicMaterial
          color="red"
          transparent
          opacity={0.005}
          side={2} // DoubleSide
          wireframe={false}
        />
      </mesh>

      {/* Yttre sfär – blå, tunn, genomskinlig */}
      <mesh>
        <sphereGeometry args={[outerScaled, 64, 64]} />
        <meshBasicMaterial
          color="green"
          transparent
          opacity={0.005}
          side={2}
          wireframe={false}
        />
      </mesh>
    </>
  )
}
