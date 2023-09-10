import './CreatorInfo.css';

export default function CreatorInfo({info}){
    return (
        <div className="inline-block">
                    <div className="col col-lg-6 mb-4 mb-lg-0">
                        <div className="card mb-3" style={{borderRadius: ".5rem"}}>
                        <div className="row g-0">
                            <div className="col-md-4 gradient-custom text-center text-white borderRadius-custom">
                            <img src={info.photo} alt="Avatar" className="img-fluid my-5" style={{width: "80px", borderRadius: "50%"}} />
                            <h5>{info.name}</h5>
                            <p>{info.profession}</p>
                            <i className="far fa-edit mb-5"></i>
                            </div>
                            <div className="col-md-8">
                            <div className="card-body p-4">
                                <h6>Information</h6>
                                <hr className="mt-0 mb-4" />
                                <div className="row pt-1">
                                <div className="col-6 mb-3">
                                    <h6>Email</h6>
                                    <p className="text-muted">{info.email}</p>
                                </div>
                                <div className="col-6 mb-3">
                                    <h6>Phone</h6>
                                    <p className="text-muted">{info.phoneNumber}</p>
                                </div>
                                </div>
                                <div className="row pt-1">
                                <div className="col-6 mb-3">
                                    <h6>Git Hub</h6>
                                    <p className="text-muted">{info.github}</p>
                                </div>
                                </div>
                                <div className="d-flex justify-content-start">
                                <a href="#!"><i className="fab fa-facebook-f fa-lg me-3"></i></a>
                                <a href="#!"><i className="fab fa-twitter fa-lg me-3"></i></a>
                                <a href="#!"><i className="fab fa-instagram fa-lg"></i></a>
                                </div>
                            </div>
                            </div>
                        </div>
                        </div>
                    </div>
        </div>
    )
}