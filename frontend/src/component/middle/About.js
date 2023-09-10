import CreatorInfo from "./about/CreatorInfo";

export default function About(){
    let info = [
        {
            name : "Nikhil Dhole",
            photo : "/nikhildhole.JPEG",
            profession : "Full Stack",
            email : "nikhildhole001@gmail.com",
            phoneNumber : "+918788325037",
            github : "https://github.com/nikhildhole"

        },
        {
            name : "Jayant Uttarwar",
            photo : "/Jayant.png",
            profession : "Full Stack",
            email : "jayantuttarwar2000@gmail.com",
            phoneNumber : "+9421513085",
            github : "https://github.com/nikhildhole"
        },

        {
             name : "Krushna Salunke",
             photo : "/krushnasalunke.jpg",
             profession : "Full Stack",
             email : "salunke.krushna8999@gmail.com",
             phoneNumber : "+919511718247",
             github : "https://github.com/Littlekrushnae"
        },
        {
            name : "Vishal Musmade",
            photo : "/vishalmusmade.jpeg",
            profession : "Full Stack",
            email : "musmadevishal1717@gmail.com",
            phoneNumber : "+919765855462",
            github : "https://github.com/vishalmusmade"
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