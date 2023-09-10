import { useEffect } from "react"
import { useNavigate } from "react-router-dom"

export default function RevisionComplete(){
    const navigate = useNavigate();
    useEffect(()=>{
        setTimeout(()=>{
            navigate("home");
        },1000)
    },[])
    return(
        <>
        <h1 className="text-center">Revision Complete</h1>
        </>
    )
}