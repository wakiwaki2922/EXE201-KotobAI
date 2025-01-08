"use client";

import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { formatCurrency } from "@/lib/utils";
import { PackagePlan } from "@/types/PackagePlan";
import React, { useEffect, useState } from "react";
import PlanDetailsModal from "./_components/PlanDetailsModal";
import { getActivePackages } from "@/actions/packages-management";

const Payments = () => {
  const [packagePlans, setPackagePlans] = useState<PackagePlan[]>([]);
  const [selectedPlan, setSelectedPlan] = useState<PackagePlan | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Fetch package plans from the backend
  useEffect(() => {
    const fetchPlans = async () => {
      try {
        const response = await getActivePackages();
        const data = response.data;
        setPackagePlans(data);
        setIsLoading(false);
      } catch (error) {
        console.error("Error fetching packages:", error);
      }
    };
    fetchPlans();
  }, []);

  const handleChoosePlan = (plan: PackagePlan) => {
    setSelectedPlan(plan);
  };

  const handleCloseModal = () => {
    setSelectedPlan(null);
  };

  return (
    <div className="p-6">
      <h2 className="text-2xl font-semibold text-center">Choose Your Plan</h2>

      <p className="text-center text-gray-600 mb-8">
        Best Plans for <strong>Your Better Study Journey</strong>
      </p>

      {isLoading ? (
        <>
          {/* Loading Spinner */}
          <div className="flex justify-center mt-8">
            <svg
              className="animate-spin h-6 w-6 text-sky-600"
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
            >
              <circle
                className="opacity-25"
                cx="12"
                cy="12"
                r="10"
                stroke="currentColor"
                strokeWidth="4"
              ></circle>
              <path
                className="opacity-75"
                fill="currentColor"
                d="M4 12a8 8 0 018-8V0c4.418 0 8 3.582 8 8s-3.582 8-8 8V4a4 4 0 00-4 4H4z"
              ></path>
            </svg>
          </div>

          {/* Skeleton Cards */}
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-6 mt-8">
            {Array(3)
              .fill(0)
              .map((_, index) => (
                <Card
                  key={index}
                  className="border rounded-lg p-6 shadow-md animate-pulse"
                >
                  <div className="h-6 bg-gray-300 rounded w-3/4 mx-auto"></div>
                  <div className="h-10 bg-gray-300 rounded w-1/2 mx-auto mt-4"></div>
                  <div className="h-4 bg-gray-300 rounded w-1/3 mx-auto mt-2"></div>
                  <Button
                    variant="default"
                    className="w-full mt-4 h-10 bg-gray-300 cursor-default"
                    disabled
                  ></Button>
                </Card>
              ))}
          </div>
        </>
      ) : (
        <>
          {/* Package Plan Cards */}
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-6">
            {packagePlans.map((plan, index) => {
              return (
                <Card
                  key={index}
                  className="border rounded-lg p-6 shadow-md transition-transform transform hover:scale-105"
                >
                  <h3 className="text-xl font-bold text-center">
                    {plan.planName}
                  </h3>
                  <p className="text-2xl text-center mt-2 font-semibold">
                    {formatCurrency(Number(plan.price))}
                  </p>
                  <p className="text-center text-sm text-gray-500 mt-1">
                    {plan.period} months
                  </p>

                  <Button
                    variant="default"
                    className="w-full mt-4"
                    onClick={() => handleChoosePlan(plan)}
                  >
                    Choose Plan
                  </Button>
                </Card>
              );
            })}
          </div>

          <div className="text-center mt-6">
            <a href="#" className="text-blue-600 underline">
              Show Detailed Plan Comparison
            </a>
          </div>
        </>
      )}

      {/* Plan Details Modal */}
      <PlanDetailsModal
        selectedPlan={selectedPlan}
        onClose={handleCloseModal}
      />
    </div>
  );
};

export default Payments;
