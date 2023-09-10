import { useSelector } from 'react-redux';
import './Home.css';

export default function Home(){
    return (
        <div className="home-container">
            <h1>Revision Tool</h1>
      <div className="home-content">
        <h4>
            Heyy, Have you revised today??
        </h4>
      </div>
    </div>

    )
}