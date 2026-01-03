import { Outlet } from "react-router-dom";
import Navbar from "./Navbar";

export default  function Layout(){
    return(
        <div className="min-h-screen w-full bg-gradient-to-br from-slate-50 to-slate-100">
            <Navbar/>
            <main className="mx-auto w-full max-w-7x1 px-4 py-8">
                <Outlet/>
            </main>
        </div>
    );
}