import React from 'react';
import { useDispatch } from 'react-redux';
import { clearTokens } from './../action/tokenSlice';
import { useSelector } from 'react-redux';
import { desetUser, setUser } from './../action/userSlice';
import { useNavigate } from 'react-router-dom';
import axios from './../axios';

export default function Profile() {
  const user = useSelector((state) => state.user);
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const token = useSelector((state) => state.token)

  const handleLogout = async () => {
    // Clear tokens from Redux store
    dispatch(clearTokens());
    // Clear user data from Redux store
    dispatch(desetUser());

    // Optionally, clear tokens from local storage or cookies as well
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');

    // Redirect the user to the login page or another desired page
    navigate('/login');
  };

  // Fetch user data on component mount
  React.useEffect(() => {
    async function fetchUserData() {
        console.log(token.accessToken);
      try {
        const response = await axios.get('/User', {
            headers: {
              Authorization: `Bearer ${token.accessToken}` // Use the token from the response
            }
          }); // Adjust the URL as needed

        if (response.data.isSuccess) {
          dispatch(setUser(response.data.data)); // Set user data in Redux store
        }
      } catch (error) {
        console.error('Error fetching user data:', error);
      }
    }

    fetchUserData();
  }, [dispatch]);

  if (!user.email) {
    // User data hasn't been fetched yet
    return <div>Loading...</div>;
  }
    return (
        <>
            <section>
                <div className="row m-2">
                    <div className="col-lg-4">
                        <div className="card mb-4 bg-secondary ">
                            <div className="card-body text-center">
                                <img src="https://mdbcdn.b-cdn.net/img/Photos/new-templates/bootstrap-chat/ava3.webp" alt="avatar"
                                    className="rounded-circle img-fluid" style={{ width: '150px' }} />
                                <h5 className="my-3">{user.firstName} {user.lastName}</h5>
                                <p className="text-muted mb-1">Full Stack Developer</p>
                                <p className="text-muted mb-4">Bay Area, San Francisco, CA</p>
                                <div className="d-flex justify-content-center mb-2">
                                    <button type="button" className="btn btn-primary">Follow</button>
                                    <button type="button" className="btn btn-outline-primary ms-1">Message</button>
                                </div>
                            </div>
                        </div>
                        <div className="card mb-4 mb-lg-0">
                            <div className="card-body p-0">
                                <ul className="list-group list-group-flush rounded-3">
                                <div className="card mb-4 mb-lg-0">
                                    <div className="card-body p-0">
                                        <div className='d-grid'>
                                            <div className='btn btn-danger text-center' onClick={handleLogout}>Logout</div>
                                        </div>
                                    </div>
                                    </div>

                                </ul>
                            </div>
                        </div>
                    </div>
                    <div className="col-lg-8">
                        <div className="card mb-4 bg-secondary">
                            <div className="card-body">
                                <div className="col-lg-8">
                                    <div className="card mb-4">
                                        <div className="card-body">
                                            <div className="row">
                                                <div className="col-sm-3">
                                                    <p className="mb-0">Full Name</p>
                                                </div>
                                                <div className="col-sm-9">
                                                    <p className="text-muted mb-0">Johnatan Smith</p>
                                                </div>
                                            </div>
                                            <hr />
                                            <div className="row">
                                                <div className="col-sm-3">
                                                    <p className="mb-0">Email</p>
                                                </div>
                                                <div className="col-sm-9">
                                                    <p className="text-muted mb-0">example@example.com</p>
                                                </div>
                                            </div>
                                            <hr />
                                            <div className="row">
                                                <div className="col-sm-3">
                                                    <p className="mb-0">Phone</p>
                                                </div>
                                                <div className="col-sm-9">
                                                    <p className="text-muted mb-0">(097) 234-5678</p>
                                                </div>
                                            </div>
                                            <hr />
                                            <div className="row">
                                                <div className="col-sm-3">
                                                    <p className="mb-0">Mobile</p>
                                                </div>
                                                <div className="col-sm-9">
                                                    <p className="text-muted mb-0">(098) 765-4321</p>
                                                </div>
                                            </div>
                                            <hr />
                                            <div className="row">
                                                <div className="col-sm-3">
                                                    <p className="mb-0">Address</p>
                                                </div>
                                                <div className="col-sm-9">
                                                    <p className="text-muted mb-0">Bay Area, San Francisco, CA</p>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-md-6">
                                <div className="card mb-4 mb-md-0 bg-secondary">
                                    <div className="card-body">
                                        <div className="col-md-6">
                                            <div className="card mb-4 mb-md-0">
                                                <div className="card-body">
                                                    <p className="mb-4">
                                                        <span className="text-primary font-italic me-1">assigment</span> Project Status
                                                    </p>
                                                    <p className="mb-1" style={{ fontSize: '.77rem' }}>Web Design</p>
                                                    <div className="progress rounded" style={{ height: '5px' }}>
                                                        <div className="progress-bar" role="progressbar" style={{ width: '80%' }} aria-valuenow="80"
                                                            aria-valuemin="0" aria-valuemax="100"></div>
                                                    </div>
                                                    <p className="mt-4 mb-1" style={{ fontSize: '.77rem' }}>Website Markup</p>
                                                    <div className="progress rounded" style={{ height: '5px' }}>
                                                        <div className="progress-bar" role="progressbar" style={{ width: '72%' }} aria-valuenow="72"
                                                            aria-valuemin="0" aria-valuemax="100"></div>
                                                    </div>
                                                    <p className="mt-4 mb-1" style={{ fontSize: '.77rem' }}>One Page</p>
                                                    <div className="progress rounded" style={{ height: '5px' }}>
                                                        <div className="progress-bar" role="progressbar" style={{ width: '89%' }} aria-valuenow="89"
                                                            aria-valuemin="0" aria-valuemax="100"></div>
                                                    </div>
                                                    <p className="mt-4 mb-1" style={{ fontSize: '.77rem' }}>Mobile Template</p>
                                                    <div className="progress rounded" style={{ height: '5px' }}>
                                                        <div className="progress-bar" role="progressbar" style={{ width: '55%' }} aria-valuenow="55"
                                                            aria-valuemin="0" aria-valuemax="100"></div>
                                                    </div>
                                                    <p className="mt-4 mb-1" style={{ fontSize: '.77rem' }}>Backend API</p>
                                                    <div className="progress rounded mb-2" style={{ height: '5px' }}>
                                                        <div className="progress-bar" role="progressbar" style={{ width: '66%' }} aria-valuenow="66"
                                                            aria-valuemin="0" aria-valuemax="100"></div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div className="col-md-6">
                                <div className="card mb-4 mb-md-0 bg-secondary">
                                    <div className="card-body ">
                                    <div className="col-md-6">
                                        <div className="card mb-4 mb-md-0">
                                            <div className="card-body">
                                            <p className="mb-4">
                                                <span className="text-primary font-italic me-1">assigment</span> Project Status
                                            </p>
                                            <p className="mb-1" style={{ fontSize: '.77rem' }}>Web Design</p>
                                            <div className="progress rounded" style={{ height: '5px' }}>
                                                <div className="progress-bar" role="progressbar" style={{ width: '80%' }} aria-valuenow="80"
                                                aria-valuemin="0" aria-valuemax="100"></div>
                                            </div>
                                            <p className="mt-4 mb-1" style={{ fontSize: '.77rem' }}>Website Markup</p>
                                            <div className="progress rounded" style={{ height: '5px' }}>
                                                <div className="progress-bar" role="progressbar" style={{ width: '72%' }} aria-valuenow="72"
                                                aria-valuemin="0" aria-valuemax="100"></div>
                                            </div>
                                            <p className="mt-4 mb-1" style={{ fontSize: '.77rem' }}>One Page</p>
                                            <div className="progress rounded" style={{ height: '5px' }}>
                                                <div className="progress-bar" role="progressbar" style={{ width: '89%' }} aria-valuenow="89"
                                                aria-valuemin="0" aria-valuemax="100"></div>
                                            </div>
                                            <p className="mt-4 mb-1" style={{ fontSize: '.77rem' }}>Mobile Template</p>
                                            <div className="progress rounded" style={{ height: '5px' }}>
                                                <div className="progress-bar" role="progressbar" style={{ width: '55%' }} aria-valuenow="55"
                                                aria-valuemin="0" aria-valuemax="100"></div>
                                            </div>
                                            <p className="mt-4 mb-1" style={{ fontSize: '.77rem' }}>Backend API</p>
                                            <div className="progress rounded mb-2" style={{ height: '5px' }}>
                                                <div className="progress-bar" role="progressbar" style={{ width: '66%' }} aria-valuenow="66"
                                                aria-valuemin="0" aria-valuemax="100"></div>
                                            </div>
                                            </div>
                                        </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </>
    );
}
