import { useLoader } from "@react-three/fiber"
import { TextureLoader } from "three"


interface StarProps {
    
    radius: number
    spectralType:string
    onHover: React.Dispatch<React.SetStateAction<boolean>>
  }
  
                 
  const EARTH_RADIUS_UNITS = 0.1           
  const SOLAR_RADIUS_IN_EARTH_RADII = 109 
  

const scaleSolarRadiusToUnits = (radiusSolarRadii: number) =>
  radiusSolarRadii * SOLAR_RADIUS_IN_EARTH_RADII * EARTH_RADIUS_UNITS


  export const Star: React.FC<StarProps> = ({ radius, spectralType, onHover }) => {
    const scaledRadius = scaleSolarRadiusToUnits(radius)
    const color = getStarColor(spectralType)
    const texture = useLoader(TextureLoader, '/textures/sun.png') 
  
    return (
      <mesh
        onPointerOver={() => onHover(true)}
        onPointerOut={() => onHover(false)}
        onPointerDown={() => onHover(true)}   
        onPointerUp={() => onHover(false)}    
      >

      {/* Inre solkroppen med textur */}
      <sphereGeometry args={[scaledRadius, 64, 64]} />
      <meshStandardMaterial
        map={texture}
        emissive={color}
        emissiveIntensity={1.5}
        toneMapped={false}
      />
    
   
    </mesh>
    )
  }

function getStarColor(spectralType: string): string {
 
    const type = spectralType?.charAt(0).toUpperCase()

 
    switch (type) {
      case 'O': return '#5b76f0' 
      case 'B': return '#ffcc6f' 
      case 'A': return '#f0f0d5' 
      case 'F': return '#f0f08d' 
      case 'G': return '#fabd16' 
      case 'K': return '#fa7d16' 
      case 'M': return '#fa1616' 
      default: return 'white'
    }
  }
