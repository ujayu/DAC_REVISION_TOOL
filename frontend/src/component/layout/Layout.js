import { Outlet, Link, useLocation } from "react-router-dom";
import { useDispatch, useSelector } from 'react-redux'
import axios from "../../axios";
import { clearTokens, selectTokens, setTokens } from '../../action/tokenSlice';

export default function Layout(){
    const role = useSelector(state => state.user.role);
    const location = useLocation();
    const isLoginPath = location.pathname !== '/login';
    const isRegisterPath = location.pathname !== '/register';


    return (
        <>
        <nav className="navbar navbar-expand-lg bg-dark sticky-top" data-bs-theme="dark">
            <div className="container-fluid">
                <Link className="navbar-brand" to="/">Revision Tool</Link>
                <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarColor02" aria-controls="navbarColor02" aria-expanded="false" aria-label="Toggle navigation">
                <span className="navbar-toggler-icon"></span>
                </button>
                <div className="collapse navbar-collapse" id="navbarColor02">
                <ul className="navbar-nav me-auto">
                    <li className={`nav-item ${location.pathname === '/' ? 'active' : ''}`}>
                        <Link className="nav-link" to="/home">Home</Link>
                    </li>
                    {role !== "" && 
                        <li className={`nav-item ${location.pathname === '/modules' ? 'active' : ''}`}>
                            <Link className="nav-link" to="/modules">Modules</Link>
                        </li>
                    }
                    {(role === "admin"  || role === "teacher") && 
                        <li className={`nav-item ${location.pathname === '/ModulesManagment' ? 'active' : ''}`}>
                            <Link className="nav-link" to="/ModulesManagment">Modules Managment</Link>
                        </li>
                    }
                    {role === "admin" && 
                        <li className={`nav-item ${location.pathname === '/admin' ? 'active' : ''}`}>
                        <Link className="nav-link" to="/admin">User Managment</Link>
                        </li>
                    }
                    <li className={`nav-item ${location.pathname === '/feedback' ? 'active' : ''}`}>
                        <Link className="nav-link" to="/feedback">Feedback</Link>
                    </li>
                    <li className={`nav-item ${location.pathname === '/about' ? 'active' : ''}`}>
                        <Link className="nav-link" to="/about">About</Link>
                    </li>
                </ul>
                <form className="d-flex">
                    {role === "" && isRegisterPath && <Link className="btn btn-secondary my-2 my-sm-0" to="/register">Register</Link>}
                    {role === "" && isLoginPath && <Link className="btn btn-primary my-2 my-sm-0" to="/login">Login</Link>}
                    {role !== "" && 
                    <Link className="nav-link" to="/profile">
                        <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" fill="currentColor" className="bi bi-person-circle" viewBox="0 0 16 16">
                            <path d="M11 6a3 3 0 1 1-6 0 3 3 0 0 1 6 0z"/>
                            <path fillRule="evenodd" d="M0 8a8 8 0 1 1 16 0A8 8 0 0 1 0 8zm8-7a7 7 0 0 0-5.468 11.37C3.242 11.226 4.805 10 8 10s4.757 1.225 5.468 2.37A7 7 0 0 0 8 1z"/>
                        </svg>
                    </Link>}
                </form>
                </div>
            </div> 
        </nav>
    
        <Outlet>

        </Outlet>
        </>
    )
}