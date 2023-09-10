import CreatorInfo from "./about/CreatorInfo";

export default function About(){
    let info = [
        {
            name : "Nikhil Dhole",
            url : "",
            profession : "Full Stack",
            email : "nikhildadadhole@gmail",
            phoneNumber : "+918788325037",
            github : "https://github.com/nikhildhole"

        },
        {
            name : "Krushna Salunke",
            url : "",
            profession : "",
            email : "nikhildadadhole@gmail",
            phoneNumber : "+918788325037",
            github : "https://github.com/nikhildhole"
        },
        {
            name : "Vishal Musmade",
            url : "",
            profession : "",
            email : "nikhildadadhole@gmail",
            phoneNumber : "+918788325037",
            github : "https://github.com/nikhildhole"
        },
        {
            name : "Purva Wagh",
            url : "",
            profession : "",
            email : "nikhildadadhole@gmail",
            phoneNumber : "+918788325037",
            github : "https://github.com/nikhildhole"
        }
    ]
    return (
        <>
         <div className="container">
            <h1>About Us</h1>
            <h4>This project made by :</h4>
            <CreatorInfo info={info[0]} />
            <CreatorInfo info={info[1]} />
            <CreatorInfo info={info[2]} />
            <CreatorInfo info={info[3]} />
        </div> 
        </>
    )
}